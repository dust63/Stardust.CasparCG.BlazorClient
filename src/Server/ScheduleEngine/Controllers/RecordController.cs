
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Stardust.Flux.ScheduleEngine.Models;
using Stardust.Flux.ScheduleEngine.Services;
using Stardust.Flux.ScheduleEngine.Factory;
using Stardust.Flux.Contract.DTO.Schedule;
using Stardust.Flux.Contract.ApiContracts;

namespace Stardust.Flux.ScheduleEngine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController : ControllerBase
    {

        private readonly IEventSchedulerService<RecordParameters> _recordService;
        public RecordController(IEventSchedulerService<RecordParameters> recordService)
        {
            _recordService = recordService;
        }


        [HttpGet]
        [Route("/manual")]
        public List<ScheduleEventResponse<RecordParameters>> GetManualRecords(int start, int limit = 100)
        {
            return _recordService
            .GetEvents(EventType.Manual, start, limit)
            .Select(x => EventJobFactory.CreateScheduleResponseDto<RecordParameters>(x))
            .ToList();
        }

        [HttpPost]
        [Route("/manual/Start")]
        public string StartRecord(RecordParameters parameters)
        {
            return _recordService.StartEventNow(parameters);
        }

        [HttpPost]
        [Route("/manual/Stop")]
        public void StopRecord(string recordJobId)
        {
            _recordService.StopEvent(recordJobId);
        }

        [HttpGet]
        [Route("/schedule")]
        public List<ScheduleEventResponse<RecordParameters>> GetSchedule(int start = 0, int limit = 100)
        {
            return _recordService
            .GetEvents(EventType.Schedule, start, limit)
            .Select(x => EventJobFactory.CreateScheduleResponseDto<RecordParameters>(x))
            .ToList();
        }


        [HttpPost]
        [Route("/schedule")]
        public ScheduleEventResponse<RecordParameters> AddSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest)
        {
            Event recordJob = _recordService.AddScheduleEvent(scheduleRecordRequest);
            return EventJobFactory.CreateScheduleResponseDto<RecordParameters>(recordJob);
        }

        [HttpPut]
        [Route("/schedule")]
        public ScheduleEventResponse<RecordParameters> UpdateSchedule(ScheduleEventDto<RecordParameters> scheduleRecordRequest)
        {
            Event recordJob = _recordService.UpdateScheduleEvent(scheduleRecordRequest);
            return EventJobFactory.CreateScheduleResponseDto<RecordParameters>(recordJob);
        }



        [HttpDelete]
        [Route("/schedule")]
        public void RemoveSchedule(string recordJobId)
        {
            _recordService.RemoveEvent(recordJobId);
        }


        [HttpGet]
        [Route("/recuring")]
        public List<RecuringEventResponse<RecordParameters>> GetRecuring(int start = 0, int limit = 0)
        {
            return _recordService
            .GetEvents(EventType.Recuring, start, limit)
            .Select(x => EventJobFactory.CreateRecuringResponseDto<RecordParameters>(x))
            .ToList();
        }

        [HttpPost]
        [Route("/recuring")]
        public RecuringEventResponse<RecordParameters> AddRecuring(RecuringEventDto<RecordParameters> recordRequest)
        {
            var job = _recordService.AddRecuringEvent(recordRequest);
            return EventJobFactory.CreateRecuringResponseDto<RecordParameters>(job);
        }

        [HttpPut]
        [Route("/recuring")]
        public RecuringEventResponse<RecordParameters> UpdateRecuring(RecuringEventDto<RecordParameters> recordRequest)
        {
            var job = _recordService.UpdateRecuringEvent(recordRequest);
            return EventJobFactory.CreateRecuringResponseDto<RecordParameters>(job);
        }

        [HttpDelete]
        [Route("/recuring")]
        public void RemoveRecuring(string jobId)
        {
            _recordService.RemoveEvent(jobId);
        }
    }
}
