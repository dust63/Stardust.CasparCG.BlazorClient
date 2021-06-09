using Elsa.Activities.File.LiquidFilter;
using Elsa.Scripting.Liquid.Extensions;

namespace Elsa.Activities.File
{
    public static class FileServiceCollectionExtensions
    {

        public static ElsaOptionsBuilder AddFTPActivities(this ElsaOptionsBuilder options)
        {

            options.Services
                .AddLiquidFilter<CombinePathFilter>("combine_path")
                .AddLiquidFilter<GetDirectoryFilter>("get_directory")
                .AddLiquidFilter<GetFilenameFilter>("get_filename");



            options
                 .AddActivity<FileCopy>()
                 .AddActivity<FileDelete>()
                 .AddActivity<FileMoved>();

            return options;
        }
    }
}
