using Elsa.Activities.FileTransferProtocol.Extensions;
using Elsa.Attributes;
using Elsa.Services;
using System.Linq;
using System.Net;

namespace Elsa.Activities.FileTransferProtocol.Activities
{

    public abstract class FtpBase : Activity
    {

        [ActivityProperty(Label = "FTP Host")]
        public string FtpHost { get; set; }

        [ActivityProperty(Label = "Username")]
        public string Username { get; set; }

        [ActivityProperty(Label = "Password")]
        public string Password { get; set; }


        [ActivityProperty(Label = "Remote path")]
        public string RemotePath { get; set; }

        protected virtual FtpWebRequest GetFtpRequest(string method, params string[] paths)
        {
            var ftpUri = $"ftp://{FtpHost}".Append(paths);
            var request = (FtpWebRequest)WebRequest.Create(ftpUri);
            request.Credentials = new NetworkCredential(Username, Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            return request;
        }

    }
}