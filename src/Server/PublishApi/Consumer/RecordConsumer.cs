using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Stardust.Flux.EventModels;

namespace Stardust.Flux.PublishApi.Consumer
{
    public class RecordStartConsumer : IConsumer<RecordStartEvent>
    {
        private readonly ILogger<RecordStartConsumer> logger;

        public RecordStartConsumer(ILogger<RecordStartConsumer> logger)
        {
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<RecordStartEvent> context)
        {
            logger.LogInformation(context.Message.JobId);
            return Task.CompletedTask;
        }
    }
}