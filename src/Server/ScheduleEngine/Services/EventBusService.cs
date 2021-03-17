using System;
using MassTransit;
using Stardust.Flux.EventModels;


namespace Stardust.Flux.ScheduleEngine.Services
{
    /// <summary>
    /// Service in charge to notify bus when event is started or stopped
    /// </summary>
    public interface IEventBusService
    {

        void NotifyForRecordStart<T>(string jobId, TimeSpan duration, T parameters);


        void NotifiyForRecordStop<T>(string jobId, TimeSpan duration, T parameters);

    }

    /// <summary>
    /// Service in charge to notify bus when event is started or stopped
    /// </summary>
    public class EventBusService : IEventBusService
    {
        private readonly IPublishEndpoint publishEndpoint;

        public EventBusService(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }


        public void NotifiyForRecordStop<T>(string jobId,  TimeSpan duration, T parameters)
        {
            publishEndpoint.Publish<EventStartMessage>(
              new EventStopMessage { JobId = jobId, Duration = duration }
                        .SerializeAndAddExtraParams<T>(parameters));
        }

        public void NotifyForRecordStart<T>(string jobId, TimeSpan duration, T parameters)
        {
            publishEndpoint.Publish<EventStartMessage>(
                new EventStartMessage { JobId = jobId , Duration = duration }
            .SerializeAndAddExtraParams<T>(parameters));
        }
    }
}