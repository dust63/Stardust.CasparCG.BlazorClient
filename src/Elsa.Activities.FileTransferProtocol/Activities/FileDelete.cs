using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elsa.Activities.FileTransferProtocol.Activities
{

    [Activity(DisplayName = "File deletion", Category = "File", Description ="Delete a file on local file system", Outcomes =new[] {"Done", "Not found" })]
    public class FileDelete : Activity
    {

        [ActivityProperty(Label = "File name", Hint = "Filename to delete on the local filesystem")]
        public string Filename { get; set; }


        protected override IActivityExecutionResult OnExecute()
        {
            if (File.Exists(Filename))
            {
                File.Delete(Filename);
                return Done(Filename);
            }

            return Outcome("Not found", Filename);             

        }
    }
}
