using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;

using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;

using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stardust.Flux.Crosscutting.Extensions;
using Hangfire;
using Stardust.Flux.Contract.DTO;
using Stardust.Flux.DataAccess;
using Stardust.Flux.Server.Options;
using Stardust.Flux.PublishApi.Youtube;

namespace Stardust.Flux.Server.Services.Youtube
{

    public abstract class BaseYoutubeService
    {
        protected readonly AuthentificationService authenticateService;

        public BaseYoutubeService(AuthentificationService authenticateService)
        {
            this.authenticateService = authenticateService;
        }
        protected async Task<YouTubeService> GetService(string accountId, CancellationToken cancellationToken, params string[] scopes)
        {
            authenticateService.CheckForAccount(accountId);
            var result = await authenticateService.AuthorizeAsync(accountId, cancellationToken, scopes);
            return new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

        }
    }
    public class YoutubeAppService : BaseYoutubeService
    {
        public ILogger<YoutubeAppService> Logger { get; }


        private readonly IOptions<YoutubeApiOptions> apiOptions;
        private readonly DataContext publishContext;
        private readonly YoutubeUploader uploader;

        public YoutubeAppService(
            ILogger<YoutubeAppService> logger,
            AuthentificationService authenticateService,
            IOptions<YoutubeApiOptions> apiOptions,
            DataContext publishContext,
            YoutubeUploader uploader) : base(authenticateService)
        {
            this.publishContext = publishContext;
            this.uploader = uploader;
            Logger = logger;
            this.apiOptions = apiOptions;
            ApiOptions = apiOptions.Value;
        }

        public TokenResponse AccessToken { get; set; }

        public YoutubeApiOptions ApiOptions { get; }


        public async Task<IDictionary<string, string>> GetAccountsInfo(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var results = await publishContext.YoutubeAccounts
            .OrderBy(x => x.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize).ToDictionaryAsync(x => x.Key, x => x.Name, cancellationToken);
            return results;
        }

        public async Task<string> AddChannelAccount(string name, CancellationToken cancellationToken)
        {
            var dataStore = new EFDataStore(publishContext, name);
            var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = ApiOptions.ClientId,
                        ClientSecret = ApiOptions.ClientSecrets
                    },
                    new[] { YouTubeService.Scope.YoutubeForceSsl },
                   dataStore.AccountId,
                     cancellationToken,
                      dataStore
                   );
            return dataStore.AccountId;
        }


        public async Task<IList<YoutubeChannelDto>> GetChannelInfo(string accountId, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeReadonly });
            var channelsRequest = service.Channels.List("id,snippet,statistics");
            channelsRequest.Mine = true;
            channelsRequest.Fields = "items";
            var response = await channelsRequest.ExecuteAsync();

            return response.Items.Select(x => new YoutubeChannelDto
            {

                Title = x.Snippet.Title,
                Description = x.Snippet.Description,
                Country = x.Snippet.Country,
                CustomUrl = x.Snippet.CustomUrl,
                DefaultLanguage = x.Snippet.DefaultLanguage,
                PublishedAtRaw = x.Snippet.PublishedAtRaw,
                Thumbnails = x.Snippet.Thumbnails.High.Url,
                SubscriberCount = x.Statistics.SubscriberCount,
                ViewCount = x.Statistics.ViewCount,
            }).ToList();
        }


        public async Task<IList<VideoCategory>> GetCategories(HttpContext context, string regionCode, string accountId, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId, cancellationToken);
            var categories = service.VideoCategories.List("snippet");
            if (!string.IsNullOrEmpty(regionCode))
                categories.RegionCode = regionCode;
            var response = await categories.ExecuteAsync(cancellationToken);
            return response.Items;
        }

        public async Task<YoutubeListItemsDto<BroadcastRequestDto>> GetLiveBroadcasts(HttpContext context, string accountId, string pageToken, int pageSize, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException($"« {nameof(accountId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(accountId));
            }

            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
            var request = service.LiveBroadcasts.List("id,snippet,contentDetails,status");
            request.PageToken = pageToken;
            request.MaxResults = pageSize;
            request.Mine = true;
            var response = await request.ExecuteAsync();
            return new YoutubeListItemsDto<BroadcastRequestDto>
            {
                PreviousPageToken = response.PrevPageToken,
                NextPageToken = response.NextPageToken,
                PageSize = response.PageInfo.ResultsPerPage ?? 0,
                TotalItems = response.PageInfo.TotalResults ?? 0,
                Items = response.Items.Select(x => CreateBroadcastRequest(x, accountId)).ToArray()
            };
        }

        public async Task<LiveBroadcast> InsertBroadcast(HttpContext context, BroadcastRequestDto broadcastRequest, CancellationToken cancellationToken)
        {
            var service = await GetService(broadcastRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var broadcast = CreateYoutubeBroadcast(broadcastRequest);
            broadcast.Id = null;
            var request = service.LiveBroadcasts.Insert(broadcast, "id,snippet,contentDetails,status");
            var resource = await request.ExecuteAsync(cancellationToken);
            return resource;
        }

        /// <summary>
        /// Update broadcast data
        /// </summary>
        /// <param name="context"></param>
        /// <param name="accountId"></param>
        /// <param name="broadcastId"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<LiveBroadcast> UpdateBroadcastStatus(HttpContext context, string accountId, string broadcastId, PrivacyStatus status, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException($"« {nameof(accountId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(accountId));
            }

            if (string.IsNullOrEmpty(broadcastId))
            {
                throw new ArgumentException($"« {nameof(broadcastId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(broadcastId));
            }

            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var broadcast = new LiveBroadcast
            {
                Id = broadcastId,
                Status = new LiveBroadcastStatus { PrivacyStatus = status.GetEnumMemberValue() }
            };
            broadcast.Id = broadcastId;
            var request = service.LiveBroadcasts.Update(broadcast, "id,status");
            var resource = await request.ExecuteAsync(cancellationToken);
            return resource;
        }

        /// <summary>
        /// Update broadcast data
        /// </summary>
        /// <param name="context"></param>
        /// <param name="broadcastId"></param>
        /// <param name="broadcastRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<LiveBroadcast> UpdateBroadcast(HttpContext context, BroadcastRequestDto broadcastRequest, CancellationToken cancellationToken)
        {
            var service = await GetService(broadcastRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var broadcast = CreateYoutubeBroadcast(broadcastRequest);
            var request = service.LiveBroadcasts.Update(broadcast, "id,snippet,contentDetails,status");
            var resource = await request.ExecuteAsync(cancellationToken);
            return resource;
        }

        public async Task<LiveBroadcast> BindLiveToBroadcast(HttpContext httpContext, string accountId, string broadcastId, string streamId, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
            var request = service.LiveBroadcasts.Bind(broadcastId, "id,snippet,contentDetails,status");
            request.StreamId = streamId;
            var response = await request.ExecuteAsync(cancellationToken);
            return response;
        }

        /// <summary>
        /// Delete a broadcast
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="accountId"></param>
        /// <param name="broadcastId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> DeleteBroadcast(HttpContext httpContext, string accountId, string broadcastId, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
            var request = service.LiveBroadcasts.Delete(broadcastId);
            var response = await request.ExecuteAsync(cancellationToken);
            return response;
        }



        public async Task<string> UploadFile(HttpContext context, UploadRequest uploadRequest, CancellationToken cancellationToken)
        {
            var service = await GetService(uploadRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
            var jobId = uploader.StoreUploadData(uploadRequest);
            BackgroundJob.Enqueue<YoutubeUploader>(uploader => uploader.UploadFile(CancellationToken.None, jobId));
            return jobId;
        }





        /// <summary>
        /// Create a live stream
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="liveRequestData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<LiveStream> InsertLiveStream(HttpContext httpContext, LiveStreamRequestDto liveRequestData, CancellationToken cancellationToken)
        {
            var service = await GetService(liveRequestData.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });

            var live = new LiveStream();
            live.Snippet = new LiveStreamSnippet
            {
                Title = liveRequestData.Title,
                Description = liveRequestData.Description
            };
            live.Cdn = new CdnSettings
            {
                FrameRate = liveRequestData.CdnFrameRate.GetEnumMemberValue(),
                Resolution = liveRequestData.CdnResolution.GetEnumMemberValue(),
                IngestionType = liveRequestData.IngestionType.GetEnumMemberValue(),
            };
            live.ContentDetails = new LiveStreamContentDetails
            {
                IsReusable = liveRequestData.IsReusable
            };
            var request = service.LiveStreams.Insert(live, "id,snippet,cdn,contentDetails,status");
            var resource = await request.ExecuteAsync(cancellationToken);
            return resource;
        }

        public async Task<LiveStream> GetLiveStream(HttpContext httpContext, string accountId, string streamId, CancellationToken cancellationToken)
        {
            if (streamId is null)
            {
                throw new ArgumentNullException(nameof(streamId));
            }

            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var request = service.LiveStreams.List("id,snippet,cdn,status");
            request.Id = streamId;
            request.MaxResults = 1;

            var response = await request.ExecuteAsync(cancellationToken);
            return response.Items.FirstOrDefault();
        }

        public async Task<LiveStreamListResponse> GetLiveStreams(HttpContext httpContext, string accountId, string pageToken, int pageSize, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var request = service.LiveStreams.List("id,snippet,cdn,status");
            request.PageToken = pageToken;
            request.Mine = true;
            request.MaxResults = pageSize;

            var response = await request.ExecuteAsync(cancellationToken);
            return response;

        }

        public async Task<string> DeleteLiveStream(HttpContext httpContext, string accountId, string liveStreamId, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var request = service.LiveStreams.Delete(liveStreamId);
            var response = await request.ExecuteAsync(cancellationToken);
            return response;
        }

        private static BroadcastRequestDto CreateBroadcastRequest(LiveBroadcast broadcast, string accountId)
        {
            var broadcastRequest = new BroadcastRequestDto();
            broadcastRequest.AccountId = accountId;
            broadcastRequest.BroadcastId = broadcast.Id;
            broadcastRequest.PrivacyStatus = (PrivacyStatus)Enum.Parse(typeof(PrivacyStatus), broadcast.Status.PrivacyStatus, true);
            broadcastRequest.SelfDeclaredMadeForKids = broadcast.Status.SelfDeclaredMadeForKids;
            broadcastRequest.EnableMonitorStream = broadcast.ContentDetails?.MonitorStream?.EnableMonitorStream;
            broadcastRequest.BroadcastStreamDelayMs = broadcast.ContentDetails?.MonitorStream?.BroadcastStreamDelayMs;
            broadcastRequest.EnableAutoStart = broadcast.ContentDetails?.EnableAutoStart;
            broadcastRequest.EnableAutoStop = broadcast.ContentDetails?.EnableAutoStop;
            broadcastRequest.EnableAutoStop = broadcast.ContentDetails?.EnableAutoStop;
            broadcastRequest.EnableClosedCaptions = broadcast.ContentDetails?.EnableClosedCaptions;
            broadcastRequest.EnableContentEncryption = broadcast.ContentDetails?.EnableContentEncryption;
            broadcastRequest.EnableDvr = broadcast.ContentDetails?.EnableDvr;
            broadcastRequest.EnableEmbed = broadcast.ContentDetails?.EnableEmbed;
            broadcastRequest.RecordFromStart = broadcast.ContentDetails?.RecordFromStart;

            broadcastRequest.ScheduleStartDate = broadcast.Snippet.ScheduledStartTime ?? DateTime.MinValue;
            broadcastRequest.ScheduleEndDate = broadcast.Snippet.ScheduledEndTime ?? DateTime.MinValue;
            broadcastRequest.Title = broadcast.Snippet.Title;
            broadcastRequest.Description = broadcast.Snippet.Description;
            broadcastRequest.ThumbnailUrl = broadcast.Snippet.Thumbnails.Default__.Url;
            return broadcastRequest;
        }

        private static LiveBroadcast CreateYoutubeBroadcast(BroadcastRequestDto broadcastRequest)
        {
            var broadcast = new LiveBroadcast();
            broadcast.Id = broadcastRequest.BroadcastId;
            broadcast.Status = new LiveBroadcastStatus
            {
                PrivacyStatus = broadcastRequest.PrivacyStatus.ToString().ToLower(),
                SelfDeclaredMadeForKids = broadcastRequest.SelfDeclaredMadeForKids
            };
            broadcast.ContentDetails = new LiveBroadcastContentDetails
            {
                MonitorStream = new MonitorStreamInfo()
            };
            broadcast.ContentDetails.MonitorStream.EnableMonitorStream = broadcastRequest.EnableMonitorStream;
            broadcast.ContentDetails.MonitorStream.BroadcastStreamDelayMs = broadcastRequest.BroadcastStreamDelayMs;
            broadcast.ContentDetails.EnableAutoStart = broadcastRequest.EnableAutoStart;
            broadcast.ContentDetails.EnableAutoStop = broadcastRequest.EnableAutoStop;
            broadcast.ContentDetails.EnableClosedCaptions = broadcastRequest.EnableClosedCaptions;
            broadcast.ContentDetails.EnableContentEncryption = broadcastRequest.EnableContentEncryption;
            broadcast.ContentDetails.EnableDvr = broadcastRequest.EnableDvr;
            broadcast.ContentDetails.EnableEmbed = broadcastRequest.EnableEmbed;
            broadcast.ContentDetails.RecordFromStart = broadcastRequest.RecordFromStart;

            broadcast.Snippet = new LiveBroadcastSnippet
            {
                ScheduledStartTime = broadcastRequest.ScheduleStartDate,
                ScheduledEndTime = broadcastRequest.ScheduleEndDate,
                Title = broadcastRequest.Title,
                Description = broadcastRequest.Description,
                Thumbnails = new ThumbnailDetails { Default__ = new Thumbnail { Url = broadcastRequest.ThumbnailUrl } }
            };
            return broadcast;
        }

    }
}