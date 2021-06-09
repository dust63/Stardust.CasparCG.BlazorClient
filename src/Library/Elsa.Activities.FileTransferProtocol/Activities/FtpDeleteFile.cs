using Elsa.ActivityResults;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa.Attributes;
using System.Net;
using Elsa.Activities.FileTransferProtocol.Models;

namespace Elsa.Activities.FileTransferProtocol.Activities
{



    [Activity(Category = "FTP", DisplayName = "FTP Delete", Description = "Delete a file or folder from a FTP server")]
    public class FtpDeleteFile : FtpBase
    {

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {                    
            var request = GetFtpRequest(WebRequestMethods.Ftp.DeleteFile, RemotePath);           

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse(); 

            return Done(new FtpActivityStatus(this, response));
        }



    }
}
