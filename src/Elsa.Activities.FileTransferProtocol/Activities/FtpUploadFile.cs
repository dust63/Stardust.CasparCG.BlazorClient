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



    [Activity(Category = "FTP", DisplayName = "FTP Upload file", Description = "Upload a file on a FTP server")]
    public class FtpUploadFile : FtpBase
    {
        [ActivityProperty(Label = "File name to upload")]
        public string FileName { get; set; }


        [ActivityProperty(Label = "Local path")]
        public string LocalPath { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            FtpWebRequest request = GetFtpRequest(WebRequestMethods.Ftp.UploadFile);

            var filePath = Path.Combine(LocalPath, Path.GetFileName(FileName));

            using (Stream fileStream = File.OpenRead(filePath))
            using (Stream ftpStream = request.GetRequestStream())
            {

                fileStream.CopyTo(ftpStream);
            }

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();


            return Done(new FtpActivityStatus(this, response) { RemoteFilename = FileName, LocalPath = LocalPath, LocalFilename = FileName });
        }

      
    }
}
