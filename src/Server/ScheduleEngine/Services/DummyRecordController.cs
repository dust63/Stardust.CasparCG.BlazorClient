using System;
using Microsoft.Extensions.Logging;
using Stardust.Flux.ScheduleEngine.DTO;
using Stardust.Flux.ScheduleEngine.Models;

namespace Stardust.Flux.ScheduleEngine.Services
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
            logger.LogInformation($"[{DateTime.Now}] Start recording {eventId}-{parameters.filePath}");
        }

     

        public void Stop(string eventId, RecordParameters parameters)
        {
            logger.LogInformation($"[{DateTime.Now}] Stop recording {eventId}");
        }

       
    }
}