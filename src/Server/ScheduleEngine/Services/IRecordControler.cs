using Stardust.Flux.ScheduleEngine.Models;

namespace Stardust.Flux.ScheduleEngine.Services
{
    public interface IRecordControler
    {
        void StartRecord(RecordJob recordJob);
        void StopRecord(RecordJob recordJob);
    }
}