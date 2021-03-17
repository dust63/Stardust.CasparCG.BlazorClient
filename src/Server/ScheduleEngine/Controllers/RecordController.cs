
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Stardust.Flux.ScheduleEngine.Models;
using Stardust.Flux.ScheduleEngine.Services;
using Stardust.Flux.ScheduleEngine.DTO;
using Stardust.Flux.ScheduleEngine.Factory;

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
        [Route("/ManualRecord")]
        public List<EventResponseDto<ScheduleEventDto>> GetManualRecords(int start, int limit = 100)
        {
            return _recordService
            .GetEvents(EventType.Manual, start, limit)
            .Select(x => EventJobFactory.CreateScheduleResponseDto(x))
            .ToList();
        }

        [HttpPost]
        [Route("/ManualRecord/Start")]
        public string StartRecord(RecordParameters parameters)
        {
            return _recordService.StartEventNow(parameters);
        }

        [HttpPost]
        [Route("/ManualRecord/Stop")]
        public void StopRecord(string recordJobId)
        {
            _recordService.StopEvent(recordJobId);
        }

        [HttpGet]
        [Route("/Schedule")]
        public List<EventResponseDto<ScheduleEventDto>> GetSchedule(int start = 0, int limit = 100)
        {
            return _recordService
            .GetEvents(EventType.Schedule, start, limit)
            .Select(x => EventJobFactory.CreateScheduleResponseDto(x))
            .ToList();
        }


        [HttpPost]
        [Route("/Schedule")]
        public EventResponseDto<ScheduleEventDto> AddSchedule(ScheduleEventDto scheduleRecordRequest)
        {
            Event recordJob = _recordService.AddScheduleEvent(scheduleRecordRequest);
            return EventJobFactory.CreateScheduleResponseDto(recordJob);
        }

        [HttpPut]
        [Route("/Schedule")]
        public EventResponseDto<ScheduleEventDto> UpdateSchedule(ScheduleEventDto scheduleRecordRequest)
        {
            Event recordJob = _recordService.UpdateScheduleEvent(scheduleRecordRequest);
            return EventJobFactory.CreateScheduleResponseDto(recordJob);
        }



        [HttpDelete]
        [Route("/Schedule")]
        public void RemoveSchedule(string recordJobId)
        {
            _recordService.RemoveEvent(recordJobId);
        }


        [HttpGet]
        [Route("/Recuring")]
        public List<EventResponseDto<RecuringEventDto>> GetRecuring(int start = 0, int limit = 0)
        {
            return _recordService
            .GetEvents(EventType.Schedule, start, limit)
            .Select(x => EventJobFactory.CreateRecuringResponseDto(x))
            .ToList();
        }

        [HttpPost]
        [Route("/Recuring")]
        public EventResponseDto<RecuringEventDto> AddRecuring(RecuringEventDto recordRequest)
        {
            var job = _recordService.AddRecuringEvent(recordRequest);
            return EventJobFactory.CreateRecuringResponseDto(job);
        }

        [HttpPut]
        [Route("/Recuring")]
        public EventResponseDto<RecuringEventDto> UpdateRecuring(RecuringEventDto recordRequest)
        {
            var job = _recordService.UpdateRecuringEvent(recordRequest);
            return EventJobFactory.CreateRecuringResponseDto(job);
        }

        [HttpDelete]
        [Route("/Recuring")]
        public void RemoveRecuring(string jobId)
        {
            _recordService.RemoveEvent(jobId);
        }
    }
}
