using System;
using Microsoft.Extensions.Logging;

namespace Stardust.Flux.ScheduleEngine.Services
{
    public interface IRecordService
    {
        void StartRecord(int slotId, string filename);


        void StopRecord(int slotId);
    }

    public class RecordService : IRecordService
    {
        private readonly ILogger<RecordService> logger;

        public RecordService(ILogger<RecordService> logger)
        {
            this.logger = logger;

        }
        public void StartRecord(int slotId, string filename)
        {
            logger.LogInformation($"[{DateTime.Now}] Start recording of {filename}");
        }

        public void StopRecord(int slotId)
        {
            logger.LogInformation($"[{DateTime.Now}] Stop recording");
        }
    }
}