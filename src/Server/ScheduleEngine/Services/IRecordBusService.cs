using System;
using MassTransit;
using Stardust.Flux.ScheduleEngine.EventModels;

namespace Stardust.Flux.ScheduleEngine.Services
{
    public interface IRecordBusService
    {

        void NotifyForRecordStart(string jobId, string fileName, int slotId);


        void NotifiyForRecordStop(string jobId, string fileName, int slotId, TimeSpan duration);

    }


    public class RecordBusService : IRecordBusService
    {
        private readonly IPublishEndpoint publishEndpoint;

        public RecordBusService(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }


        public void NotifiyForRecordStop(string jobId, string fileName, int slotId, TimeSpan duration)
        {
            publishEndpoint.Publish<RecordStartEvent>(
              new RecordStopEvent { JobId = jobId }
                        .AddExtraParams("FileName", fileName)
                        .AddExtraParams("SlotId", slotId)
                        .AddExtraParams("Duration", duration));
        }

        public void NotifyForRecordStart(string jobId, string fileName, int slotId)
        {
            publishEndpoint.Publish<RecordStartEvent>(
                new RecordStartEvent { JobId = jobId }
            .AddExtraParams("FileName", fileName)
            .AddExtraParams("SlotId", slotId));
        }
    }
}