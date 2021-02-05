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

        public async Task<LiveBroadcast> InsertBroadcast(HttpContext context, BroadcastRequestDto broadcastRequest, CancellationToken cancellationToken)
        {
            var service = await GetService(context, broadcastRequest.AccountId, cancellationToken, new[] { YouTubeService.Scope.Youtube });
            var broadcast = new LiveBroadcast();
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
            var request = service.LiveBroadcasts.Insert(broadcast, "id,snippet,contentDetails,status");
            var resource = await request.ExecuteAsync(cancellationToken);
            return resource;
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
    }
}