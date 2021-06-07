using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Activities.FileTransferProtocol.Activities
{

    [Activity(DisplayName = "FTP List", Description ="List file and direcotry on a ftp path", Category = "FTP")]
    public class FtpList : FtpBase
    {
     

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var request = GetFtpRequest(WebRequestMethods.Ftp.ListDirectoryDetails, RemotePath);

            string[] list;
            using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                list = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return Done(list);
        }
      
    }
}
