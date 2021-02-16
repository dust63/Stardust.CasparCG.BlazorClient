using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly IRecordSchedulerService _recordService;
        public RecordController(IRecordSchedulerService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet]
        [Route("/ManualRecord")]
        public List<RecordResponseDto<ScheduleRecordJobDto>> GetManualRecords(int start, int limit = 100)
        {
            return _recordService
            .GetRecordJob(RecordJobType.Manual, start, limit)
            .Select(x => RecordJobFactory.CreateScheduleResponseDto(x))
            .ToList();
        }

        [HttpPost]
        [Route("/ManualRecord/Start")]
        public string StartRecord(string fileName, int slotId)
        {
            return _recordService.StartRecordNow(fileName, slotId);
        }

        [HttpPost]
        [Route("/ManualRecord/Stop")]
        public void StopRecord(string recordJobId)
        {
            _recordService.StopRecord(recordJobId);
        }

        [HttpGet]
        [Route("/Schedule")]
        public List<RecordResponseDto<ScheduleRecordJobDto>> GetSchedule(int start = 0, int limit = 100)
        {
            return _recordService
            .GetRecordJob(RecordJobType.Schedule, start, limit)
            .Select(x => RecordJobFactory.CreateScheduleResponseDto(x))
            .ToList();
        }


        [HttpPost]
        [Route("/Schedule")]
        public RecordResponseDto<ScheduleRecordJobDto> AddSchedule(ScheduleRecordJobDto scheduleRecordRequest)
        {
            RecordJob recordJob = _recordService.AddSchedule(scheduleRecordRequest);
            return RecordJobFactory.CreateScheduleResponseDto(recordJob);
        }

        [HttpPut]
        [Route("/Schedule")]
        public RecordResponseDto<ScheduleRecordJobDto> UpdateSchedule(ScheduleRecordJobDto scheduleRecordRequest)
        {
            RecordJob recordJob = _recordService.UpdateSchedule(scheduleRecordRequest);
            return RecordJobFactory.CreateScheduleResponseDto(recordJob);
        }



        [HttpDelete]
        [Route("/Schedule")]
        public void RemoveSchedule(string recordJobId)
        {
            _recordService.RemoveRecordJob(recordJobId);
        }


        [HttpGet]
        [Route("/Recuring")]
        public List<RecordResponseDto<RecuringRecordJobDto>> GetRecuring(int start = 0, int limit = 0)
        {
            return _recordService
            .GetRecordJob(RecordJobType.Schedule, start, limit)
            .Select(x => RecordJobFactory.CreateRecuringResponseDto(x))
            .ToList();
        }

        [HttpPost]
        [Route("/Recuring")]
        public RecordResponseDto<RecuringRecordJobDto> AddRecuring(RecuringRecordJobDto recordRequest)
        {
            RecordJob job = _recordService.AddRecuring(recordRequest);
            return RecordJobFactory.CreateRecuringResponseDto(job);
        }

        [HttpPut]
        [Route("/Recuring")]
        public RecordResponseDto<RecuringRecordJobDto> UpdateRecuring(RecuringRecordJobDto recordRequest)
        {
            RecordJob job = _recordService.UpdateRecuring(recordRequest);
            return RecordJobFactory.CreateRecuringResponseDto(job);
        }

        [HttpDelete]
        [Route("/Recuring")]
        public void RemoveRecuring(string jobId)
        {
            _recordService.RemoveRecordJob(jobId);
        }
    }
}
