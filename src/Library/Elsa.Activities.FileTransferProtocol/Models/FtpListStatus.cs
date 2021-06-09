using Elsa.Activities.FileTransferProtocol.Activities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Elsa.Activities.FileTransferProtocol.Models
{
    class FtpListStatus : FtpActivityStatus
    {

        public FtpListStatus(FtpBase activity, FtpWebResponse response):base(activity, response)
        {
           
        }
        public string[] DirectoryList { get; set; }
    }
}
