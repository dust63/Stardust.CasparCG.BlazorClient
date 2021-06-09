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
    DisplayName = "Obs start streaming",
    Description = "Use obs websocket to start streaming")]
    public class ObsStartStreaming : ObsBaseActivity
    {
        public ObsStartStreaming(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            if (OBSWebsocket.GetStreamingStatus().IsStreaming)
                OBSWebsocket.StopStreaming();
            OBSWebsocket.StartStreaming();
            return Done();
        }
    }
}
