using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stardust.Flux.PublishApi.Models;
using Stardust.Flux.Crosscutting;
using Stardust.Flux.Crosscutting.Extensions;

namespace Stardust.Flux.PublishApi.Youtube
{
    public interface IYoutubeAppService
    {
        Task<string> AddChannelAccount(string name, CancellationToken cancellationToken);
        Task<IDictionary<string, string>> GetAccountsInfo(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<IList<VideoCategory>> GetCategories(HttpContext context, string regionCode, string accountId, CancellationToken cancellationToken);
        Task<IList<Channel>> GetChannelInfo(HttpContext context, string accountId, CancellationToken cancellationToken);
        Task UploadFile(HttpContext context, UploadRequest uploadRequest, CancellationToken cancellationToken);
    }

    public class YoutubeAppService : IYoutubeAppService
    {
        public ILogger<YoutubeAppService> Logger { get; }

        private readonly AuthenticateService authenticateService;
        private readonly PublishContext publishContext;
        public YoutubeAppService(
            ILogger<YoutubeAppService> logger,
            AuthenticateService authenticateService,
            IOptions<YoutubeApiOptions> apiOptions,
            PublishContext context)
        {
            this.publishContext = context;
            this.Logger = logger;
            this.authenticateService = authenticateService;
            ApiOptions = apiOptions.Value;
        }

        public TokenResponse AccessToken { get; set; }

        public YoutubeApiOptions ApiOptions { get; }

        private async Task<YouTubeService> GetService(HttpContext httpContext, string accountId, CancellationToken cancellationToken, params string[] scopes)
        {
            authenticateService.CheckForAccount(accountId);
            var result = await authenticateService.AuthorizeAsync(httpContext, accountId, cancellationToken, scopes);
            return new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });
        }

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


        public async Task<IList<Channel>> GetChannelInfo(HttpContext context, string accountId, CancellationToken cancellationToken)
        {
            var service = await GetService(context, accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeReadonly });
            var channelsRequest = service.Channels.List("id,snippet,statistics");
            channelsRequest.Mine = true;
            channelsRequest.Fields = "items";
            var response = await channelsRequest.ExecuteAsync();
            return response.Items;
        }


        public async Task<IList<VideoCategory>> GetCategories(HttpContext context, string regionCode, string accountId, CancellationToken cancellationToken)
        {
            var service = await GetService(context, accountId, cancellationToken);
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

            var service = await GetService(context, accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
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
            var service = await GetService(context, broadcastRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
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

            var service = await GetService(context, accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
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
            var service = await GetService(context, broadcastRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var broadcast = CreateYoutubeBroadcast(broadcastRequest);
            var request = service.LiveBroadcasts.Update(broadcast, "id,snippet,contentDetails,status");
            var resource = await request.ExecuteAsync(cancellationToken);
            return resource;
        }

        public async Task<LiveBroadcast> BindLiveToBroadcast(HttpContext httpContext, string accountId, string broadcastId, string streamId, CancellationToken cancellationToken)
        {
            var service = await GetService(httpContext, accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
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
            var service = await GetService(httpContext, accountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });
            var request = service.LiveBroadcasts.Delete(broadcastId);
            var response = await request.ExecuteAsync(cancellationToken);
            return response;
        }



        public async Task UploadFile(HttpContext context, UploadRequest uploadRequest, CancellationToken cancellationToken)
        {
            var service = await GetService(context, uploadRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.YoutubeUpload });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = uploadRequest.Title;
            video.Snippet.Description = uploadRequest.Description;
            video.Snippet.Tags = uploadRequest.Tags;
            video.Snippet.CategoryId = uploadRequest.CategoryId;
            video.Status = new VideoStatus { PrivacyStatus = uploadRequest.PrivacyStatus.ToString().ToLower() };


            var youtubeUploadState = new YoutubeUpload { YoutubeAccountId = uploadRequest.AccountId, FilePath = uploadRequest.FilePath };
            publishContext.YoutubeUploads.Add(youtubeUploadState);
            publishContext.SaveChanges();
            using (var fileStream = new FileStream(uploadRequest.FilePath, FileMode.Open))
            {
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.NotifySubscribers = UploadRequest.NotifySubscribers;
                videosInsertRequest.ProgressChanged += (e) => videosInsertRequest_ProgressChanged(e, youtubeUploadState.YoutubeUploadId);
                videosInsertRequest.ResponseReceived += (e) => videosInsertRequest_ResponseReceived(e, youtubeUploadState.YoutubeUploadId);
                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress, string uploadStateId)
        {
            var uploadState = publishContext.YoutubeUploads.Single(x => x.YoutubeUploadId == uploadStateId);
            uploadState.State = progress.Status.ToString();
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Logger.LogInformation("{0} bytes sent.", progress.BytesSent);
                    uploadState.BytesSent = progress.BytesSent;
                    break;
                case UploadStatus.Failed:
                    Logger.LogError("An error prevented the upload from completing.\n{0}", progress.Exception);
                    uploadState.Error = progress.Exception.ToString();
                    break;
            }
            publishContext.SaveChanges();
        }


        void videosInsertRequest_ResponseReceived(Video video, string uploadStateId)

        {
            Logger.LogInformation("Video:{0} published on youtube.", video.Id);
            var uploadState = publishContext.YoutubeUploads.Single(x => x.YoutubeUploadId == uploadStateId);
            uploadState.VideoId = video.Id;
            publishContext.SaveChanges();
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
            var service = await GetService(httpContext, liveRequestData.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });

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

            var service = await GetService(httpContext, accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var request = service.LiveStreams.List("id,snippet,cdn,status");
            request.Id = streamId;
            request.MaxResults = 1;

            var response = await request.ExecuteAsync(cancellationToken);
            return response.Items.FirstOrDefault();
        }

        public async Task<LiveStreamListResponse> GetLiveStreams(HttpContext httpContext, string accountId, string pageToken, int pageSize, CancellationToken cancellationToken)
        {
            var service = await GetService(httpContext, accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var request = service.LiveStreams.List("id,snippet,cdn,status");
            request.PageToken = pageToken;
            request.Mine = true;
            request.MaxResults = pageSize;

            var response = await request.ExecuteAsync(cancellationToken);
            return response;

        }

        public async Task<string> DeleteLiveStream(HttpContext httpContext, string accountId, string liveStreamId, CancellationToken cancellationToken)
        {
            var service = await GetService(httpContext, accountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
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
            broadcastRequest.ThumbnailDetails = broadcast.Snippet.Thumbnails;
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
                Thumbnails = broadcastRequest.ThumbnailDetails
            };
            return broadcast;
        }

    }
}