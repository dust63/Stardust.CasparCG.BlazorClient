using Elsa;
using Elsa.Activities.FileTransferProtocol.Activities;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class FtpServiceCollectionExtensions
    {

        public static ElsaOptionsBuilder AddFTPActivities(this ElsaOptionsBuilder options)
        {
            options
                   .AddActivity<FtpDownloadFile>()
                   .AddActivity<FtpDeleteFile>()
                   .AddActivity<FtpUploadFile>()
                   .AddActivity<FtpList>();

   

            return options;
        }
    }
}
