using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using Stardust.Flux.ScheduleEngine.DTO;
using Stardust.Flux.ScheduleEngine.Factory;
using Stardust.Flux.ScheduleEngine.Models;


namespace Stardust.Flux.ScheduleEngine.Services
{

    public interface IRecordSchedulerService
    {
        void StopAllMissedStop();
        List<RecordJob> GetRecordJob(string type, int start, int limit);
        RecordJob GetRecordJob(string recordJobId);
        RecordJob AddRecuring(RecuringRecordJobDto recordRequest);
        RecordJob UpdateRecuring(RecuringRecordJobDto recordRequest);
        void RemoveRecordJob(string recordJobId);
        RecordJob AddSchedule(ScheduleRecordJobDto recordRequest);
        RecordJob UpdateSchedule(ScheduleRecordJobDto recordRequest);
        void StartRecord(string recordJobId);
        string StartRecordNow(string fileName, int slotId);
        void StopRecord(string recordJobId);
        void AbortRecord(string recordJobId);
    }

    public class RecordSchedulerService : IRecordSchedulerService
    {
        private readonly ILogger<RecordSchedulerService> logger;
        private readonly IRecordControler recordControler;
        private readonly IRecordBusService recordBusService;
        private readonly ScheduleContext scheduleContext;

        public RecordSchedulerService(
            ILogger<RecordSchedulerService> logger,
            IRecordControler recordControler,
            IRecordBusService recordBusService,
            ScheduleContext scheduleContext)
        {
            this.logger = logger;
            this.recordControler = recordControler;
            this.recordBusService = recordBusService;
            this.scheduleContext = scheduleContext;
        }


        public void StopAllMissedStop()
        {
            var missedJobs = scheduleContext.RecordJobs
            .Where(x => x.IsRecording)
            .AsEnumerable()
            .Where(missedJob => missedJob.LastExecution.Value.Add(missedJob.Duration) < DateTime.UtcNow)
            .ToArray();

            if (!missedJobs.Any())
                return;

            logger.LogWarning($"{missedJobs.Length} record job are waiting to be stop");
            foreach (var missedJob in missedJobs)
            {
                AbortRecord(missedJob.RecordJobId);
            }
        }

        public RecordJob AddRecuring(RecuringRecordJobDto recordRequest)
        {
            RecuringPrecheck(recordRequest);
            var recordJob = RecordJobFactory.CreateEntity(recordRequest);

            RecurringJob.AddOrUpdate<IRecordSchedulerService>(recordJob.StartRecordJobId, x => x.StartRecord(recordJob.RecordJobId), recordRequest.CronExpression);
            scheduleContext.Add(recordJob);
            scheduleContext.SaveChanges();
            return recordJob;
        }

        public RecordJob UpdateRecuring(RecuringRecordJobDto recordRequest)
        {
            RecuringPrecheck(recordRequest);

            var recordJob = GetRecordJob(recordRequest.Id);
            var needToReschedule = recordJob.CronExpression != recordRequest.CronExpression;
            RecordJobFactory.UpdateEntity(recordJob, recordRequest);
            if (needToReschedule)
            {
                recordJob.CronExpression = recordRequest.CronExpression;
                RecurringJob.AddOrUpdate<IRecordSchedulerService>(recordJob.StartRecordJobId, x => x.StartRecord(recordJob.RecordJobId), recordJob.CronExpression);
            }
            scheduleContext.Update(recordJob);
            scheduleContext.SaveChanges();
            return recordJob;
        }

        private static void RecuringPrecheck(RecuringRecordJobDto recordRequest)
        {
            if (recordRequest is null)
            {
                throw new ArgumentNullException(nameof(recordRequest));
            }

            if (string.IsNullOrWhiteSpace(recordRequest.CronExpression))
            {
                throw new ArgumentNullException(nameof(recordRequest.CronExpression));
            }
        }


        public RecordJob UpdateSchedule(ScheduleRecordJobDto recordRequest)
        {
            ShcedulePrecheck(recordRequest);

            var recordJob = GetRecordJob(recordRequest.Id);


            var needToRescheduleStart = recordJob.ScheduleAt != recordRequest.ScheduleAt;
            var needToRescheduleStop = recordJob.StopRecordJobId != null && recordJob.Duration.TotalSeconds != recordRequest.DurationSeconds;

            RecordJobFactory.UpdateEntity(recordJob, recordRequest);
            if (needToRescheduleStart)
            {
                if (recordRequest.ScheduleAt < DateTime.UtcNow)
                    throw new ScheduleDateOutdatedException();

                logger.LogInformation($"Reschedule start for {recordJob.RecordJobId}. Stop Job id: {recordJob.StartRecordJobId}");
                BackgroundJob.Delete(recordJob.StartRecordJobId);
                recordJob.StartRecordJobId = BackgroundJob.Schedule<IRecordSchedulerService>(x => x.StartRecord(recordJob.RecordJobId), recordJob.ScheduleAt.Value);
            }

            if (needToRescheduleStop)
            {
                logger.LogInformation($"Reschedule stop for {recordJob.RecordJobId}. Stop Job id: {recordJob.StopRecordJobId}");
                BackgroundJob.Delete(recordJob.StopRecordJobId);
                if (recordJob.ScheduleAt.Value.Add(recordJob.Duration) < DateTime.UtcNow)
                    recordJob.StopRecordJobId = BackgroundJob.Enqueue<IRecordSchedulerService>(x => x.StopRecord(recordJob.RecordJobId));
                else
                    recordJob.StopRecordJobId = ScheduleStopRecord(recordJob);
            }

            scheduleContext.Update(recordJob);
            scheduleContext.SaveChanges();
            return recordJob;
        }


        public RecordJob AddSchedule(ScheduleRecordJobDto recordRequest)
        {
            ShcedulePrecheck(recordRequest);
            if (recordRequest.ScheduleAt < DateTime.UtcNow)
            {
                throw new ScheduleDateOutdatedException();
            }
            var recordJob = RecordJobFactory.CreateEntity(recordRequest);

            var startJobId = BackgroundJob.Schedule<IRecordSchedulerService>(x => x.StartRecord(recordJob.RecordJobId), recordRequest.ScheduleAt);
            recordJob.StartRecordJobId = startJobId;
            scheduleContext.Add(recordJob);
            scheduleContext.SaveChanges();
            return recordJob;
        }

        private static void ShcedulePrecheck(ScheduleRecordJobDto recordRequest)
        {
            if (recordRequest is null)
            {
                throw new ArgumentNullException(nameof(recordRequest));
            }

            if (recordRequest.ScheduleAt == DateTime.MinValue)
            {
                throw new ArgumentNullException(nameof(recordRequest.ScheduleAt));
            }

        }

        public void RemoveRecordJob(string recordJobId)
        {
            var recordJob = GetRecordJob(recordJobId);
            if (recordJob.IsRecording)
                AbortRecord(recordJobId);

            if (recordJob.ScheduleAt != null)
                BackgroundJob.Delete(recordJob.StartRecordJobId);
            else
                RecurringJob.RemoveIfExists(recordJob.StartRecordJobId);

            scheduleContext.Remove(recordJob);
            scheduleContext.SaveChanges();
        }

        public RecordJob GetRecordJob(string recordJobId)
        {
            var recordJob = scheduleContext.RecordJobs.SingleOrDefault(x => x.RecordJobId == recordJobId);
            if (recordJob == null)
                throw new NoRecordJobFoundException(recordJobId);
            return recordJob;
        }


        public void StartRecord(string recordJobId)
        {
            var recordJob = GetRecordJob(recordJobId);
            try
            {
                recordControler.StartRecord(recordJob);
                recordJob.StopRecordJobId = ScheduleStopRecord(recordJob);
                recordJob.IsRecording = true;

                recordBusService.NotifyForRecordStart(
                    recordJob.RecordJobId,
                    recordJob.Filename,
                    recordJob.RecordSlotId);

            }
            catch (Exception e)
            {
                recordJob.LastError = e.Message;
                throw;
            }
            finally
            {
                recordJob.LastExecution = DateTime.UtcNow;
                scheduleContext.Update(recordJob);
                scheduleContext.SaveChanges();
            }
        }

        public string StartRecordNow(string fileName, int slotId)
        {
            var recordJob = new RecordJob
            {
                RecordJobId = Guid.NewGuid().ToString(),
                Name = $"Manual record @{DateTime.UtcNow}",
                Filename = fileName,
                RecordSlotId = slotId,
                ScheduleAt = DateTime.UtcNow,
                Duration = TimeSpan.FromDays(1),
                RecordType = RecordJobType.Manual
            };
            scheduleContext.RecordJobs.Add(recordJob);
            scheduleContext.SaveChanges();

            var jobId = BackgroundJob.Enqueue<IRecordSchedulerService>(x => x.StartRecord(recordJob.RecordJobId));
            recordJob.StartRecordJobId = jobId;
            scheduleContext.SaveChanges();

            return recordJob.RecordJobId;
        }

        private static string ScheduleStopRecord(RecordJob recordJob)
        {
            var recordJobId = recordJob.RecordJobId;
            var stopTimestamp = recordJob.ScheduleAt == null ?
                DateTime.UtcNow.Add(recordJob.Duration) :
                recordJob.ScheduleAt.Value.Add(recordJob.Duration);

            var stopJobId = BackgroundJob.Schedule<IRecordSchedulerService>(x => x.StopRecord(recordJobId), stopTimestamp);
            return stopJobId;
        }


        public void StopRecord(string recordJobId)
        {
            var recordJob = GetRecordJob(recordJobId);
            if (recordJob == null)
                throw new NoRecordJobFoundException(recordJobId);
            recordControler.StopRecord(recordJob);
            BackgroundJob.Delete(recordJob.StopRecordJobId);

            recordJob.IsRecording = false;
            recordJob.StopRecordJobId = null;
            scheduleContext.Update(recordJob);
            scheduleContext.SaveChanges();

            recordBusService.NotifiyForRecordStop(
                recordJob.RecordJobId,
                 recordJob.Filename,
                  recordJob.RecordSlotId,
                   recordJob.Duration);
        }


        public void AbortRecord(string recordJobId)
        {
            var recordJob = GetRecordJob(recordJobId);
            if (!recordJob.IsRecording)
                throw new NotRecordingException();

            if (!string.IsNullOrEmpty(recordJob.StopRecordJobId))
                BackgroundJob.Delete(recordJob.StopRecordJobId);

            BackgroundJob.Enqueue<IRecordSchedulerService>(x => x.StopRecord(recordJobId));
        }

        public List<RecordJob> GetRecordJob(string type, int start, int limit)
        {
            return scheduleContext.RecordJobs
            .Where(x => x.RecordType == type)
            .Skip(start * 0).Take(limit)
            .ToList();
        }
    }
}