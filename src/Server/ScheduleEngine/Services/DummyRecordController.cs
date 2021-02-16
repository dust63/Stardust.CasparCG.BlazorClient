using System;
using Microsoft.Extensions.Logging;
using Stardust.Flux.ScheduleEngine.Models;

namespace Stardust.Flux.ScheduleEngine.Services
{
    public class DummyRecordController : IRecordControler
    {
        private readonly ILogger<DummyRecordController> logger;

        public DummyRecordController(ILogger<DummyRecordController> logger)
        {
            this.logger = logger;
        }

        public void StartRecord(RecordJob recordJob)
        {
            logger.LogInformation($"[{DateTime.Now}] Start recording {recordJob.RecordJobId}-{recordJob.RecordType}-{recordJob.Filename}");
        }

        public void StopRecord(RecordJob recordJob)
        {
            logger.LogInformation($"[{DateTime.Now}] Stop recording {recordJob.RecordJobId}");
        }
    }
}