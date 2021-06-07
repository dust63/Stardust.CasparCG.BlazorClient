using Elsa.Activities.ObsStudio.Activities;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Elsa.Activities.ObsStudio.Activities
{
    [Activity(
     Category = "OBS",
     DisplayName = "Obs wait for media ended",
     Description = "Waiting for a source media is ended")]
    public class ObsWaitForMediaEnded : ObsBaseActivity
    {
        private readonly ObsWebsocketMediaListener _listener;

        public ObsWaitForMediaEnded(ObsWebsocketInstanceFactory obsWebSocketInstanceService, ObsWebsocketMediaListener listener) : base(obsWebSocketInstanceService)
        {
            this._listener = listener;
        }

        [ActivityProperty(
            Hint = "Source name of media to wait for trigger next", 
            Label = "Source name", 
            SupportedSyntaxes =new[]{SyntaxNames.JavaScript, SyntaxNames.Liquid} 
            )]
        public string SourceName { get; set; }



        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            
            if (!context.WorkflowExecutionContext.IsFirstPass)
            {
                _listener.Listen(OBSWebsocket, context,SourceName);
                return Suspend();
            }           
            return Done(context.Input);
        }


        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
         
            return Done(context.Input);
        }    

    
    }
}
