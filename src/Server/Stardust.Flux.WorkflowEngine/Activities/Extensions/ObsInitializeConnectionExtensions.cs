using Elsa.Builders;
using Elsa.Services.Models;
using OBSWebsocketDotNet;
using System;
using System.Threading.Tasks;

namespace Stardust.Flux.WorkflowEngine.Activities
{
    public static class ObsInitializeConnectionExtensions
    {
        public static ISetupActivity<ObsInitializeConnection> UseHost(this ISetupActivity<ObsInitializeConnection> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.ObsWebSocketHost, value);
        public static ISetupActivity<ObsInitializeConnection> UseHost(this ISetupActivity<ObsInitializeConnection> activity, Func<string> value) => activity.Set(x => x.ObsWebSocketHost, value);
        public static ISetupActivity<ObsInitializeConnection> UseHost(this ISetupActivity<ObsInitializeConnection> activity, string value) => activity.Set(x => x.ObsWebSocketHost, value);



        public static ISetupActivity<ObsInitializeConnection> UsePort(this ISetupActivity<ObsInitializeConnection> activity, Func<ActivityExecutionContext, uint> value) => activity.Set(x => x.ObsWebSocketPort, value);
        public static ISetupActivity<ObsInitializeConnection> UsePort(this ISetupActivity<ObsInitializeConnection> activity, Func<uint> value) => activity.Set(x => x.ObsWebSocketPort, value);
        public static ISetupActivity<ObsInitializeConnection> UsePort(this ISetupActivity<ObsInitializeConnection> activity, uint value) => activity.Set(x => x.ObsWebSocketPort, value);


        public static ISetupActivity<ObsInitializeConnection> UsePassword(this ISetupActivity<ObsInitializeConnection> activity, Func<ActivityExecutionContext, string> value) => activity.Set(x => x.ObsWebSocketPassword, value);
        public static ISetupActivity<ObsInitializeConnection> UsePassword(this ISetupActivity<ObsInitializeConnection> activity, Func<string> value) => activity.Set(x => x.ObsWebSocketPassword, value);
        public static ISetupActivity<ObsInitializeConnection> UsePassword(this ISetupActivity<ObsInitializeConnection> activity, string value) => activity.Set(x => x.ObsWebSocketPassword, value);
               


    }
}
