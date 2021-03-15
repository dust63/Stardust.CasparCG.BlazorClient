using System.Threading.Tasks;
using MassTransit;
using Stardust.Flux.PublishApi.EventModels;

namespace PublishApi.Consumer
{
    public class RecordConsumer : IConsumer<RecordStartEvent>
    {
        public Task Consume(ConsumeContext<RecordStartEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}