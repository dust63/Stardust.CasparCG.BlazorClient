using Elsa;
using Elsa.Activities.FileTransferProtocol.Activities;
using Elsa.Activities.FileTransferProtocol.LiquidFilter;
using Elsa.Scripting.Liquid.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FtpServiceCollectionExtensions
    {

        public static ElsaOptionsBuilder AddFTPActivities(this ElsaOptionsBuilder options)
        {

            options.Services
                .AddLiquidFilter<CombinePathFilter>("combine_path")
                .AddLiquidFilter<GetPathFilter>("get_path")
                .AddLiquidFilter<GetFilenameFilter>("get_filename");

            options
                   .AddActivity<FtpDownloadFile>()
                   .AddActivity<FtpDeleteFile>()
                   .AddActivity<FtpUploadFile>()
                   .AddActivity<FtpList>();

            options
                 .AddActivity<FileCopy>()
                 .AddActivity<FileDelete>()
                 .AddActivity<FileMoved>();

            return options;
        }
    }
}
