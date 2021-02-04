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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stardust.Flux.PublishApi.Models;

namespace Stardust.Flux.PublishApi.Youtube
{
    public class YoutubeAppService
    {
        public ILogger<YoutubeAppService> Logger { get; }
        private readonly PublishContext context;
        public YoutubeAppService(
            ILogger<YoutubeAppService> logger,
            IOptions<YoutubeApiOptions> apiOptions,
            PublishContext context)
        {
            this.context = context;
            this.Logger = logger;
            ApiOptions = apiOptions.Value;
        }

        public TokenResponse AccessToken { get; set; }

        public YoutubeApiOptions ApiOptions { get; }



        private async Task<YouTubeService> GetService(string accountId, params string[] scopes)
        {
            var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        new ClientSecrets
                        {
                            ClientId = ApiOptions.ClientId,
                            ClientSecret = ApiOptions.ClientSecrets
                        },
                        scopes.Any() ? scopes : new[] { YouTubeService.Scope.YoutubeForceSsl },
                        accountId,
                         CancellationToken.None,
                         new EFDataStore(context));


            return new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });
        }

        public async Task<IDictionary<string, string>> GetAccountInfo(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var results = await context.YoutubeAccounts
            .OrderBy(x => x.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize).ToDictionaryAsync(x => x.Key, x => x.Name, cancellationToken);
            return results;
        }

        public async Task<string> AddChannelAccount(string name, CancellationToken cancellationToken)
        {
            var dataStore = new EFDataStore(context, name);
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



        public async Task<IList<Channel>> GetChannelInfo(string accountId)
        {
            var service = await GetService(accountId);
            var channelsRequest = service.Channels.List("snippet");
            var response = await channelsRequest.ExecuteAsync();
            return response.Items;
        }


        public async Task<IList<VideoCategory>> GetCategories(string regionCode, string accountId, CancellationToken cancellationToken)
        {
            var service = await GetService(accountId);
            var categories = service.VideoCategories.List("snippet");
            if (!string.IsNullOrEmpty(regionCode))
                categories.RegionCode = regionCode;
            var response = await categories.ExecuteAsync(cancellationToken);
            return response.Items;
        }




        public async Task UploadFile(UploadRequest uploadRequest, CancellationToken cancellationToken)
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = uploadRequest.Title;
            video.Snippet.Description = uploadRequest.Description;
            video.Snippet.Tags = uploadRequest.Tags;
            video.Snippet.CategoryId = uploadRequest.CategoryId;
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = uploadRequest.PrivacyStatus;


            using (var fileStream = new FileStream(uploadRequest.FilePath, FileMode.Open))
            {
                var service = await GetService(uploadRequest.ChannelId);
                var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Logger.LogInformation("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Logger.LogError("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Logger.LogInformation("Video:{0} published on youtube.", video.Id);
        }
    }
}