using System;
using Microsoft.Extensions.Logging;
using Stardust.Flux.Contract.DTO.Schedule;
using Stardust.Flux.ScheduleEngine.Services;

namespace Stardust.Flux.Server.Services
{
    public class DummyRecordController : IEventConsumer<RecordParameters>
    {
        private readonly ILogger<DummyRecordController> logger;

        public DummyRecordController(ILogger<DummyRecordController> logger)
        {
            this.logger = logger;
        }

        public void Start(string eventId, TimeSpan duration, RecordParameters parameters)
        {
            logger.LogInformation($"[{DateTime.Now}] Start recording {eventId}-{parameters.fileName}");
        }



        public void Stop(string eventId, RecordParameters parameters)
        {
            logger.LogInformation($"[{DateTime.Now}] Stop recording {eventId}");
        }


    }
}