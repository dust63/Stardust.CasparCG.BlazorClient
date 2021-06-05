using Elsa.Builders;
using Elsa.Services.Models;
using System;

namespace Stardust.Flux.WorkflowEngine.Activities
{



    public static class ObsSetCurrentSceneExtensions
    {

        public static ISetupActivity<ObsSetCurrentScene> UseSceneIndex(this ISetupActivity<ObsSetCurrentScene> activity, Func<ActivityExecutionContext, uint> value) => activity.Set(x => x.SceneIndex, value);
        public static ISetupActivity<ObsSetCurrentScene> UseSceneIndex(this ISetupActivity<ObsSetCurrentScene> activity, Func<uint> value) => activity.Set(x => x.SceneIndex, value);
        public static ISetupActivity<ObsSetCurrentScene> UseSceneIndex(this ISetupActivity<ObsSetCurrentScene> activity, uint value) => activity.Set(x => x.SceneIndex, value);


        public static ISetupActivity<ObsSetCurrentScene> UseSceneName(this ISetupActivity<ObsSetCurrentScene> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.SceneName, value);
        public static ISetupActivity<ObsSetCurrentScene> UseSceneName(this ISetupActivity<ObsSetCurrentScene> activity, Func<string> value) => activity.Set(x => x.SceneName, value);
        public static ISetupActivity<ObsSetCurrentScene> UseSceneName(this ISetupActivity<ObsSetCurrentScene> activity, string value) => activity.Set(x => x.SceneName, value);          

    }

    public static class ObsSetFFMPEGMediaSourceSettingsExtensions
    {


        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithFile(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.MediaFilename, value);
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithFile(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<string> value) => activity.Set(x => x.MediaFilename, value);
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithFile(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, string value) => activity.Set(x => x.MediaFilename, value);


        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithLoop(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<ActivityExecutionContext, bool> value) => activity.Set(x => x.Looping, (c) => new bool?(value(c)));
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithLoop(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<bool> value) => activity.Set(x => x.Looping, () => new bool?(value()));
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithLoop(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, bool value) => activity.Set(x => x.Looping, value);

        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithHardwareDecode(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<ActivityExecutionContext, bool> value) => activity.Set(x => x.HWDecode, (c) => new bool?(value(c)));
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithHardwareDecode(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<bool> value) => activity.Set(x => x.HWDecode,()=> new bool?( value()));
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> WithHardwareDecode(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, bool value) => activity.Set(x => x.HWDecode, value);


        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> UseSourceName(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.SourceName, value);
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> UseSourceName(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, Func<string> value) => activity.Set(x => x.SourceName, value);
        public static ISetupActivity<ObsSetFFMPEGMediaSourceSettings> UseSourceName(this ISetupActivity<ObsSetFFMPEGMediaSourceSettings> activity, string value) => activity.Set(x => x.SourceName, value);

    }

  
}
