using Refit;
using Stardust.Flux.Contract.ApiContracts;
using Stardust.Flux.Contract.DTO.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{

    [Headers("Content-Type: application/json")]
    public interface IRecordClientApi
    {


        [Get("/manual")]
        Task<ApiResponse<List<ScheduleEventResponse<RecordParameters>>>> GetManualRecords(int start, int limit = 100);

        [Get("/recuring")]
        Task<ApiResponse<List<RecuringEventResponse<RecordParameters>>>> GetRecuring(int start = 0, int limit = 100);

        [Get("/schedule")]
        Task<ApiResponse<List<ScheduleEventResponse<RecordParameters>>>> GetSchedule(int start = 0, int limit = 100);



        ScheduleEventResponse<RecordParameters> AddRecuring(RecuringEventDto<RecordParameters> recordRequest);
        ScheduleEventResponse<RecordParameters> AddSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest);

        void RemoveRecuring(string jobId);
        void RemoveSchedule(string recordJobId);
        string StartRecord(RecordParameters parameters);
        void StopRecord(string recordJobId);
        ScheduleEventResponse<RecordParameters> UpdateRecuring(RecuringEventDto<RecordParameters> recordRequest);
        ScheduleEventResponse<RecordParameters> UpdateSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest);

    }
}
