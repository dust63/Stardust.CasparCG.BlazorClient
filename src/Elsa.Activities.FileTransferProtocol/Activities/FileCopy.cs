using Elsa.Activities.FileTransferProtocol.Extensions;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using System.IO;

namespace Elsa.Activities.FileTransferProtocol.Activities
{
    [Activity(DisplayName = "File copy", Category = "File", Description = "Copy a file on local file system", Outcomes = new[] { "Done", "Not found" })]
    public class FileCopy : Activity
    {

        [ActivityProperty(Label = "File path to copy", Hint = "File path to copy on the local filesystem")]
        public string SourceFilePath { get; set; }


        [ActivityProperty(Label = "Destination directory", Hint = "Direcotry where we want to copy")]
        public string DestinationDirectory { get; set; }


        protected override IActivityExecutionResult OnExecute()
        {
            if (File.Exists(SourceFilePath))
            {
                var destinationPath = Path.Combine(DestinationDirectory, Path.GetFileName(SourceFilePath));
                destinationPath.EnsureDirectoryExists();
                File.Copy(SourceFilePath, destinationPath);
                return Done(destinationPath);
            }

            return Outcome("Not found", SourceFilePath);

        }
    }
}
