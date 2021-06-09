using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;
using Elsa.Activities.ObsStudio.Activities;
using System.Threading.Tasks;

namespace Elsa.Activities.ObsStudio.Activities
{
    [Activity(Category = "OBS", DisplayName = "Obs start streaming", Description = "Use obs websocket to start streaming")]
    public abstract class ObsBaseActivity : Activity
    {

        protected ObsWebsocketInstanceFactory _obsWebsocketInstanceService { get; set; }

        public OBSWebsocket OBSWebsocket { get; private set; }


        public ObsBaseActivity(ObsWebsocketInstanceFactory obsWebSocketInstanceService)
        {
            _obsWebsocketInstanceService = obsWebSocketInstanceService;
        }



        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            OBSWebsocket = _obsWebsocketInstanceService.GetInstance(context.WorkflowInstance);
            return base.OnExecute(context);
        }

        protected override ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            OBSWebsocket = _obsWebsocketInstanceService.GetInstance(context.WorkflowInstance);
            return base.OnExecuteAsync(context);
        }

        public override ValueTask<IActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {

            OBSWebsocket = _obsWebsocketInstanceService.GetInstance(context.WorkflowInstance);
            return base.ExecuteAsync(context);
        }



    }
}