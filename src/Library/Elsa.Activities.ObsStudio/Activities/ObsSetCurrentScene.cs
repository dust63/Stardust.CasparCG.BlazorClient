using Elsa.Activities.ObsStudio.Activities;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;
using System;
using System.Linq;

namespace Elsa.Activities.ObsStudio.Activities
{
    [Activity(
     Category = "OBS",
     DisplayName = "Obs activate scene",
     Description = "Use obs websocket to activate a scene",
     Outcomes = new[] { "Done", "SceneNotFound" })]
    public class ObsSetCurrentScene : ObsBaseActivity
    {
        public ObsSetCurrentScene(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {
        }

        [ActivityProperty(Hint = "Scene name to activate", Label = "Scene Name", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string SceneName { get; set; }

        [ActivityProperty(Hint = "Scene index to activate",  Label = "Scene Index", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public uint SceneIndex { get; set; }

        protected override bool OnCanExecute(ActivityExecutionContext context) => SceneName != null || SceneIndex > 0;

      

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            base.OnExecute(context);
            return ActivateBySceneName() ?? ActivateBySceneIndex();
        }

        public IActivityExecutionResult ActivateBySceneIndex()
        {
            var sceneToActive = OBSWebsocket.GetSceneList().Scenes.ElementAtOrDefault((int)SceneIndex - 1);
            if (sceneToActive.Name == null)
                return Outcome("SceneNotFound");


            OBSWebsocket.SetCurrentScene(sceneToActive.Name);


            return Done(sceneToActive.Name);
        }

        public IActivityExecutionResult ActivateBySceneName()
        {
            if (string.IsNullOrWhiteSpace(SceneName))
            {
                return null;
            }

            try
            {
                OBSWebsocket.SetCurrentScene(SceneName);
            }
            catch (Exception e)
            {

                return Outcome("SceneNotFound", e);
            }

            return Done(SceneName);
        }
    }
}
