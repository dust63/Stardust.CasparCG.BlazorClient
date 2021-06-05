using Elsa.Builders;
using Elsa.Services.Models;
using System;

namespace Stardust.Flux.WorkflowEngine.Activities
{
    public static class ObsSetSourceVisibilityExtensions
    {
        public static ISetupActivity<ObsSetSourceVisibility> UseSourceName(this ISetupActivity<ObsSetSourceVisibility> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.SourceName, value);
        public static ISetupActivity<ObsSetSourceVisibility> UseSourceName(this ISetupActivity<ObsSetSourceVisibility> activity, Func<string> value) => activity.Set(x => x.SourceName, value);
        public static ISetupActivity<ObsSetSourceVisibility> UseSourceName(this ISetupActivity<ObsSetSourceVisibility> activity, string value) => activity.Set(x => x.SourceName, value);

        public static ISetupActivity<ObsSetSourceVisibility> UseSourceIndex(this ISetupActivity<ObsSetSourceVisibility> activity, Func<ActivityExecutionContext, uint> value) => activity.Set(x => x.SourceIndex, value);
        public static ISetupActivity<ObsSetSourceVisibility> UseSourceIndex(this ISetupActivity<ObsSetSourceVisibility> activity, Func<uint> value) => activity.Set(x => x.SourceIndex, value);
        public static ISetupActivity<ObsSetSourceVisibility> UseSourceIndex(this ISetupActivity<ObsSetSourceVisibility> activity, uint value) => activity.Set(x => x.SourceIndex, value);


        public static ISetupActivity<ObsSetSourceVisibility> UseSceneName(this ISetupActivity<ObsSetSourceVisibility> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.SceneName, value);
        public static ISetupActivity<ObsSetSourceVisibility> UseSceneName(this ISetupActivity<ObsSetSourceVisibility> activity, Func<string> value) => activity.Set(x => x.SceneName, value);
        public static ISetupActivity<ObsSetSourceVisibility> UseSceneName(this ISetupActivity<ObsSetSourceVisibility> activity, string value) => activity.Set(x => x.SceneName, value);

    }
}
