using Elsa.Activities.ObsStudio.Activities;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Configuration;
using OBSWebsocketDotNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elsa.Activities.ObsStudio.Activities
{


    [Activity(
   Category = "OBS",
   DisplayName = "Obs start recording",
   Description = "Use obs websocket to start recording")]
    public class ObsStartRecording : ObsBaseActivity
    {
        public ObsStartRecording(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            if (OBSWebsocket.GetRecordingStatus().IsRecording)
                OBSWebsocket.StopRecording();
            OBSWebsocket.StartRecording();

            return Done();
        }
    }
}
