using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
using Stardust.Flux.Crosscutting.Extensions;
using Stardust.Flux.DataAccess;
using Stardust.Flux.DataAccess.Models;
using Stardust.Flux.PublishApi.Youtube;

namespace Stardust.Flux.Server.Services.Youtube
{
    public class YoutubeUploader : BaseYoutubeService
    {
        private readonly ILogger<YoutubeUploader> logger;
        private DataContext publishContext;
        private readonly YoutubeSignalRClient signalRClient;


        public string CurrentJobId { get; private set; }
        public long CurrentFileSize { get; private set; }

        private VideosResource.InsertMediaUpload videosInsertRequest;

        public YoutubeUploader(
            ILogger<YoutubeUploader> logger,
            DataContext publishContext,
            YoutubeSignalRClient signalRClient,
            AuthentificationService authenticateService) : base(authenticateService)
        {
            this.logger = logger;
            this.publishContext = publishContext;
            this.signalRClient = signalRClient;
        }

        public string StoreUploadData(UploadRequest uploadRequest)
        {
            var youtubeUploadState = new YoutubeUpload
            {
                YoutubeAccountId = uploadRequest.AccountId,
                FilePath = uploadRequest.FilePath,
                Title = uploadRequest.Title,
                Description = uploadRequest.Description,
                CategoryId = uploadRequest.CategoryId,
                PrivacyStatus = uploadRequest.PrivacyStatus.GetEnumMemberValue(),
                Tags = uploadRequest.Tags != null ? string.Join(",", uploadRequest.Tags) : null,
            };
            publishContext.YoutubeUploads.Add(youtubeUploadState);
            publishContext.SaveChanges();

            return youtubeUploadState.YoutubeUploadId;
        }

        public async Task UploadFile(CancellationToken cancellationToken, string jobId)
        {
            CurrentJobId = jobId;
            var request = publishContext.YoutubeUploads.Single(x => x.YoutubeUploadId == jobId);
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = request.Title;
            video.Snippet.Description = request.Description;
            video.Snippet.Tags = request.Tags?.Split(',');
            video.Snippet.CategoryId = request.CategoryId;
            video.Status = new VideoStatus { PrivacyStatus = request.PrivacyStatus };
            var service = await GetService(request.YoutubeAccountId, CancellationToken.None, YouTubeService.Scope.YoutubeUpload);

            using (var fileStream = new FileStream(request.FilePath, FileMode.Open))
            {
                CurrentFileSize = fileStream.Length;
                videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.NotifySubscribers = UploadRequest.NotifySubscribers;
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;
                try
                {
                    await videosInsertRequest.UploadAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error during upload to youtube of job:{request.YoutubeUploadId}");
                }
                finally
                {
                    videosInsertRequest.ProgressChanged -= videosInsertRequest_ProgressChanged;
                    videosInsertRequest.ResponseReceived -= videosInsertRequest_ResponseReceived;
                    videosInsertRequest = null;
                }
            }
        }



        private void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {

            var uploadState = publishContext.YoutubeUploads.Single(x => x.YoutubeUploadId == CurrentJobId);
            uploadState.State = progress.Status.ToString();

            signalRClient.ReportStatus(CurrentJobId, progress.Status, CancellationToken.None);
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    logger.LogInformation("{0} bytes sent.", progress.BytesSent);
                    uploadState.BytesSent = progress.BytesSent;
                    signalRClient.ReportProgress(CurrentJobId, progress.BytesSent, CurrentFileSize, CancellationToken.None);
                    break;
                case UploadStatus.Failed:
                    logger.LogError("An error prevented the upload from completing.\n{0}", progress.Exception);
                    uploadState.Error = progress.Exception.ToString();
                    signalRClient.ReportError(CurrentJobId);
                    break;
                case UploadStatus.Completed:
                    signalRClient.ReportCompletion(CurrentJobId);
                    break;
            }
            publishContext.SaveChanges();
        }


        private void videosInsertRequest_ResponseReceived(Video video)
        {
            logger.LogInformation("Video:{0} published on youtube.", video.Id);
            var uploadState = publishContext.YoutubeUploads.Single(x => x.YoutubeUploadId == CurrentJobId);
            uploadState.YoutubeVideoId = video.Id;
            publishContext.SaveChanges();
        }
    }
}