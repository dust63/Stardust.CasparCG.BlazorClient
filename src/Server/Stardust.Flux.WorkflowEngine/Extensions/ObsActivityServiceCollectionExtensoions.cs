using Elsa;
using Stardust.Flux.WorkflowEngine.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ObsActivityServiceCollectionExtensoions
    {

        public static ElsaOptionsBuilder AddObsActivities(this ElsaOptionsBuilder options)
        {
            options.Services
                      .AddSingleton<ObsWebsocketInstanceFactory>()
                      .AddScoped<ObsWebsocketMediaListener>();

            options
                   .AddActivity<ObsInitializeConnection>()
                   .AddActivity<ObsStartStreaming>()
                   .AddActivity<ObsStopStreaming>()
                   .AddActivity<ObsStartRecording>()
                   .AddActivity<ObsStopRecording>()
                   .AddActivity<ObsSetCurrentScene>()
                   .AddActivity<ObsWaitForMediaEnded>()
                   .AddActivity<ObsSetSourceVisibility>()
                   .AddActivity<ObsSetFFMPEGMediaSourceSettings>();

            return options;
        }
    }
}
