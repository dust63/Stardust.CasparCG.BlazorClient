using Stardust.Flux.Contract.DTO.Schedule;
using System.Collections.Generic;

namespace Stardust.Flux.Contract.ApiContracts
{
    public interface IRecordApi
    {
        ScheduleEventResponse<RecordParameters> AddRecuring(RecuringEventDto<RecordParameters> recordRequest);
        ScheduleEventResponse<RecordParameters> AddSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest);
        
        List<ScheduleEventResponse<RecordParameters>> GetManualRecords(int start, int limit = 100);
        List<ScheduleEventResponse<RecordParameters>> GetRecuring(int start = 0, int limit = 0);

        List<ScheduleEventResponse<RecordParameters>> GetSchedule(int start = 0, int limit = 100);
        void RemoveRecuring(string jobId);
        void RemoveSchedule(string recordJobId);
        string StartRecord(RecordParameters parameters);
        void StopRecord(string recordJobId);
        ScheduleEventResponse<RecordParameters> UpdateRecuring(RecuringEventDto<RecordParameters> recordRequest);
        ScheduleEventResponse<RecordParameters> UpdateSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest);
    }
}