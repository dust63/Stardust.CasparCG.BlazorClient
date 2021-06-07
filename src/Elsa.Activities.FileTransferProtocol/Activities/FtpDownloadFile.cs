using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Attributes;
using System.IO;
using System.Net;
using System;
using Elsa.Activities.FileTransferProtocol.Extensions;
using Elsa.Activities.FileTransferProtocol.Models;

namespace Elsa.Activities.FileTransferProtocol.Activities
{



    [Activity(Category = "FTP", DisplayName = "FTP Download file", Description = "Dowload a file from a FTP server")]
    public class FtpDownloadFile : FtpBase
    {
        [ActivityProperty(Label = "File name to download")]
        public string FileName { get; set; }


        [ActivityProperty(Label = "Local path")]
        public string LocalPath { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var filePath = Path.Combine(LocalPath, Path.GetFileName(FileName));
            var rootPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            FtpWebRequest request = GetFtpRequest(WebRequestMethods.Ftp.DownloadFile, RemotePath, FileName); 

            using var response = (FtpWebResponse)request.GetResponse();
            using Stream inStream = response.GetResponseStream();
            using Stream outStream = File.OpenWrite(filePath);
         
            inStream.CopyTo(outStream);

            return Done(new FtpActivityStatus(this, response) { RemoteFilename = FileName,LocalPath = LocalPath, LocalFilename = FileName });
        }



    }
}
