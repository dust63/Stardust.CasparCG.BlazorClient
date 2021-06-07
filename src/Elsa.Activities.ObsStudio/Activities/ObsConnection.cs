using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Configuration;
using OBSWebsocketDotNet;

namespace Elsa.Activities.ObsStudio.Activities
{

    [Activity(Category = "OBS", DisplayName = "Obs Connection",Description = "Initialize websocket connection to OBS studio. Use the plugin from https://github.com/Palakis/obs-websocket", Outcomes = new[] { "Done", "Error" })]
    public class ObsInitializeConnection : Activity
    {
        public IConfiguration Configuration { get; }

        public ObsInitializeConnection(IConfiguration configuration, ObsWebsocketInstanceFactory obsWebsocketInstanceService)
        {
            Configuration = configuration;
            ObsInstanceService = obsWebsocketInstanceService;
        }

        public ObsWebsocketInstanceFactory ObsInstanceService { get; set; }




        [ActivityProperty(Hint = "localhost", DefaultValue = "localhost", Label = "Hostname", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string ObsWebSocketHost { get; set; }


        [ActivityProperty(Hint = "4444", DefaultValue = 4444, Label = "Port", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public uint ObsWebSocketPort { get; set; }

        [ActivityProperty(Hint = "mypassword", Label = "Obs websocket password", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string ObsWebSocketPassword { get; set; }


        [ActivityProperty(DefaultValue = false, Label = "Use configuration file", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool UseConfiguration { get; set; }



        public string ObsWebSocketUrl { get; set; }
        protected void InitializeConnection(WorkflowInstance workflowInstance)
        {



            if (UseConfiguration)
            {
                ObsWebSocketHost = Configuration["Obs.Websocket.Host"];
                ObsWebSocketPort = uint.Parse(Configuration["Obs.Websocket.Port"]);
                ObsWebSocketPassword = Configuration["Obs.Websocket.Password"];
                return;
            }

            ObsWebSocketUrl = $"ws://{ObsWebSocketHost}:{ObsWebSocketPort}";
            ObsInstanceService.GetOrStoreInstance(workflowInstance, ObsWebSocketUrl, ObsWebSocketPassword);
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {                    
            try
            {
                InitializeConnection(context.WorkflowInstance);
                return Done(ObsWebSocketUrl);
            }
            catch (System.Exception e)
            {

                return Outcome("Error", e);
            }
        }

    }
}