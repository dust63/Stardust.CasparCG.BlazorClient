using Elsa.Activities.FileTransferProtocol.Activities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Elsa.Activities.FileTransferProtocol.Models
{
    [Serializable]
    [DataContract]
    public class FtpActivityStatus
    {

        public FtpActivityStatus()
        {

        }


        public FtpActivityStatus(FtpBase ftpActivity, FtpWebResponse response)
        {
            FtpHost = ftpActivity.FtpHost;
            FtpUser = ftpActivity.Username;
            RemotePath = ftpActivity.RemotePath;          
            ResponseMessage = response.StatusDescription;
            ResponseMessage = response.StatusCode.ToString();
        }
        [DataMember]
        public string FtpHost { get; set; }

        [DataMember]
        public string FtpUser { get; set; }
        [DataMember]
        public string RemoteFilename { get; set; }

        [DataMember]
        public string RemotePath { get; set; }

        [DataMember]
        public string LocalFilename { get; set; }

        [DataMember]
        public string LocalPath { get; set; }

        [DataMember]
        public string ResponseMessage { get; set; }

        [DataMember]
        public string ResponseStatusCode { get; set; }

    }
}
