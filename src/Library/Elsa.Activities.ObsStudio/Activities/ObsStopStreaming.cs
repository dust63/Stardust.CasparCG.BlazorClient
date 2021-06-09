using Elsa.Activities.ObsStudio.Activities;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;

namespace Elsa.Activities.ObsStudio.Activities
{
    [Activity(
    Category = "OBS",
    DisplayName = "Obs stop streaming",
    Description = "Use obs websocket to stop streaming")]
    public class ObsStopStreaming : ObsBaseActivity
    {
        public ObsStopStreaming(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            if (OBSWebsocket.GetStreamingStatus().IsStreaming)
                OBSWebsocket.StopStreaming();
            return Done();
        }
    }
}
