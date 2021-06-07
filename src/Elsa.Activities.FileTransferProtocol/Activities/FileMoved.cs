using Elsa.Activities.FileTransferProtocol.Extensions;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using System.IO;

namespace Elsa.Activities.FileTransferProtocol.Activities
{
    [Activity(DisplayName = "File moved/rename", Category = "File", Description = "Rename a file on local file system", Outcomes = new[] { "Done", "Not found" })]
    public class FileMoved : Activity
    {

        [ActivityProperty(Label = "File path to rename", Hint = "File path to rename on the local filesystem")]
        public string SourceFilePath { get; set; }


        [ActivityProperty(Label = "Destination file path", Hint = "Final file path")]
        public string DestinationFilePath { get; set; }


        protected override IActivityExecutionResult OnExecute()
        {
            if (File.Exists(SourceFilePath))
            {
                DestinationFilePath.EnsureDirectoryExists();
                File.Move(SourceFilePath, DestinationFilePath);
                return Done(DestinationFilePath);
            }

            return Outcome("Not found", SourceFilePath);

        }
    }
}
