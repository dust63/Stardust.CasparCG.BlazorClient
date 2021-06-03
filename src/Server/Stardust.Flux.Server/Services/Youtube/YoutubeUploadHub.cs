using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Upload;
using Microsoft.AspNetCore.SignalR;

namespace Stardust.Flux.Server.Services.Youtube
{
    public class YoutubeUploadHub : Hub
    {

        public Task ReportProgress(string uploadJobId, long progress, long total, CancellationToken cancellationToken)
        {
            return Clients.All.SendAsync("progress", uploadJobId, progress, total, cancellationToken);
        }


        public Task ReportStatus(string uploadJobId, UploadStatus status, CancellationToken cancellationToken)
        {
            return Clients.All.SendAsync("status", uploadJobId, status, cancellationToken);
        }



        public Task ReportCompletion(string uploadJobId)
        {
            return Clients.All.SendAsync("completed", uploadJobId);
        }

        public Task ReportError(string uploadJobId)
        {
            return Clients.All.SendAsync("error", uploadJobId);
        }
    }
}