using Elsa.Activities.Console;
using Elsa.Activities.Temporal;
using Elsa.Builders;
using NodaTime;
using Elsa.Activities.ObsStudio.Activities.Extensions;

namespace Stardust.Flux.WorkflowEngine.Workflow
{
    public class StreamingEventWorkflow : IWorkflow
    {

        private readonly Instant _executeAt;
        private readonly Duration _duration;

        public StreamingEventWorkflow(Instant executeAt, NodaTime.Duration duration)
        {
            _executeAt = executeAt;
            _duration = duration;
        }

        public void Build(IWorkflowBuilder builder)
        {
            builder
                
                .StartAt(_executeAt)
                .ObsConnect().WithName("ObsConnection")
                .ObsSetCurrentScene(1).WithName("SetScene")                 
                .ObsStartStreaming().WithName("StartStreaming").WithDisplayName("Start Streaming")
                .StartIn(Duration.FromSeconds(10))
                .ObsSetCurrentScene("Void").WithName("SetScene2")
                .StartIn(_duration)
                .ObsStopStreaming().WithName("StopStreaming").WithDisplayName("Stop Streaming");
        }
    }
}
