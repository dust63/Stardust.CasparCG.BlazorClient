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
using ScheduleEngine.Models;
using ScheduleEngine.Services;

namespace ScheduleEngine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController : ControllerBase
    {

        private readonly ILogger<RecordController> _logger;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public RecordController(ILogger<RecordController> logger,
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _logger = logger;
        }


        [HttpPost]
        [Route("/StartRecord")]
        public string StartRecord(string duration, string fileName, int slotId)
        {
            var durationTimespan = TimeSpan.Parse(duration);
            var id = _backgroundJobClient.Enqueue<IRecordService>(rec => rec.StartRecord(slotId, fileName));
            return _backgroundJobClient.Schedule<IRecordService>(rec => rec.StopRecord(slotId), durationTimespan);
        }

        [HttpPost]
        [Route("/StopRecord")]
        public string StopRecord(int slotId, string jobId)
        {
            _backgroundJobClient.Delete(jobId);
            return _backgroundJobClient.Enqueue<IRecordService>(rec => rec.StopRecord(slotId));
        }

        [HttpGet]
        [Route("/Schedule")]
        public List<ScheduleRecordJob> GetSchedule(int start = 0, int limit = 100)
        {
            var api = JobStorage.Current.GetMonitoringApi();
            var scheduledJobs = api.ScheduledJobs(start, limit);
            return scheduledJobs.Select(x => new ScheduleRecordJob { Id = x.Key, ScheduleAt = x.Value.ScheduledAt }).ToList();
        }


        [HttpPost]
        [Route("/Schedule")]
        public string AddSchedule(DateTimeOffset startDate, string duration, string fileName, int slotId)
        {
            return _backgroundJobClient
            .Schedule<IRecordService>((rec) => rec.StartRecord(slotId, fileName), startDate);
        }

        [HttpDelete]
        [Route("/Schedule")]
        public void RemoveSchedule(string jobId)
        {
            _backgroundJobClient.Delete(jobId);
            _logger.LogInformation($"{jobId} unscheduled");
        }


        [HttpGet]
        [Route("/Recuring")]
        public List<RecuringRecordJob> GetRecuring(int start = 0, int limit = 0)
        {
            var connection = JobStorage.Current.GetConnection();
            var api = limit > 0 ? connection.GetRecurringJobs().Skip(0 * limit).Take(limit) : connection.GetRecurringJobs();
            return JobStorage.Current.GetConnection().GetRecurringJobs().Select(x => new RecuringRecordJob
            {
                Id = x.Id,
                CronExpression = x.Cron,
                LastExecution = x.LastExecution,
                NextExecution = x.NextExecution,
                LastError = x.Error
            }).ToList();
        }

        [HttpPost]
        [Route("/Recuring")]
        public string AddOrUpdateRecuring(string identifier, string cronExpression, string duration, string fileName, int slotId)
        {
            var id = identifier ?? Guid.NewGuid().ToString();
            _recurringJobManager
         .AddOrUpdate<IRecordService>(
             id,
            (rec) => rec.StartRecord(slotId, fileName),
             cronExpression);
            return id;
        }

        [HttpDelete]
        [Route("/Recuring")]
        public void RemoveRecuring(string jobId)
        {
            _recurringJobManager.RemoveIfExists(jobId);
            _logger.LogInformation($"{jobId} unscheduled");
        }

    }
}
