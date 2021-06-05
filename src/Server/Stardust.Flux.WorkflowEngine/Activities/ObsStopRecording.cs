using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;

namespace Stardust.Flux.WorkflowEngine.Activities
{
    [Activity(
     Category = "OBS",
     DisplayName = "Obs stop recording",
     Description = "Use obs websocket to stop recording",
     Outcomes= new[]{"Done","NotRecording"})]
    public class ObsStopRecording : ObsBaseActivity
    {
        public ObsStopRecording(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {
        }

        public string RecordFileName { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            var recordStatus = OBSWebsocket.GetRecordingStatus();
            if (recordStatus.IsRecording)
            {
                OBSWebsocket.StopRecording();
                RecordFileName = recordStatus.RecordingFilename;
                return Done(RecordFileName);
            }

            return Outcome("NotRecording");           
        }
    }
}
