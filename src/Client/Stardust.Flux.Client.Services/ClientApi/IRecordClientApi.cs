using Refit;
using Stardust.Flux.Contract.DTO.Schedule;
using System.Collections.Generic;
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


        [Post("/recuring")]
        ScheduleEventResponse<RecordParameters> AddRecuring(RecuringEventDto<RecordParameters> recordRequest);

        [Post("/schedule")]
        ScheduleEventResponse<RecordParameters> AddSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest);

        [Delete("/recuring")]
        Task<IApiResponse> RemoveRecuring(string jobId);


        [Delete("/schedule")]
        Task<IApiResponse> RemoveSchedule(string recordJobId);

        [Post("/manual/start")]
        Task<IApiResponse> StartRecord(RecordParameters parameters);


        [Post("/manual/stop")]
        Task<IApiResponse> StopRecord(string recordJobId);


        [Put("/recuring")]
        Task<ApiResponse<ScheduleEventResponse<RecordParameters>>> UpdateRecuring(RecuringEventDto<RecordParameters> recordRequest);

        [Put("/schedule")]
        Task<ApiResponse<ScheduleEventResponse<RecordParameters>>> UpdateSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest);

    }
}
