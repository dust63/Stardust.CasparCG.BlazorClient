using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;
using System;
using System.Linq;

namespace Stardust.Flux.WorkflowEngine.Activities
{
    [Activity(
     Category = "OBS",
     DisplayName = "Obs set source visibility",
     Description = "Set visibility of a source in a scene",
     Outcomes = new[] { "Done", "SceneNotFound", "SourceNotFound" })]
    public class ObsSetSourceVisibility : ObsBaseActivity
    {
        public ObsSetSourceVisibility(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {
        }



        [ActivityProperty(Hint = "Scene name to activate",  Label = "Scene Name", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string SceneName { get; set; }

        [ActivityProperty(Hint = "Source name to activate", Label = "Source Name", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string SourceName { get; set; }

        [ActivityProperty(Hint = "Source index to activate",  Label = "Source Index", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public uint SourceIndex { get; set; }

        [ActivityProperty(Hint = "Is source will be visible", Label = "Source Visible", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool Visibile { get; set; }

        protected override bool OnCanExecute(ActivityExecutionContext context) => SourceName != null || SourceIndex > 0;

      

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            base.OnExecute(context);
            if (string.IsNullOrWhiteSpace(SceneName))
            {
                return Outcome("SceneNotFound");
            }
            return ActivateBySourceName() ?? ActivateBySourceIndex();
        }

        public IActivityExecutionResult ActivateBySourceIndex()
        {      
            var source = OBSWebsocket.GetSceneItemList(SceneName).ElementAtOrDefault((int)SourceIndex - 1);      


            OBSWebsocket.SetSourceRender(source.SourceName, Visibile, SceneName);
            return Done(source.SourceName);
        }

        public IActivityExecutionResult ActivateBySourceName()
        {
            if (string.IsNullOrWhiteSpace(SourceName))
                return null;

            //var sceneItems = OBSWebsocket.GetSceneItemList(SceneName);
            //var source = sceneItems.FirstOrDefault(x => x.SourceName == SourceName);
      

            OBSWebsocket.SetSourceRender(SourceName, Visibile, SceneName);           
            return Done(SourceName);
        }
    }
}
