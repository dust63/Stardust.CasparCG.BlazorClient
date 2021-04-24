using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Stardust.Flux.Contract.DTO.Schedule;
using Stardust.Flux.ScheduleEngine.Factory;
using Stardust.Flux.ScheduleEngine.Models;


namespace Stardust.Flux.ScheduleEngine.Services
{
    /// <summary>
    /// A class that manage scheduling event
    /// </summary>
    public interface IEventSchedulerService<T>
    {
        /// <summary>
        /// Stop all event that are missed to stop
        /// </summary>
        void StopAllMissedStop();

        /// <summary>
        /// Get a list of events
        /// </summary>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        List<Event> GetEvents(string type, int start, int limit);

        /// <summary>
        /// Get a specific event
        /// </summary>
        /// <param name="eventJobId"></param>
        /// <returns></returns>
        Event GetEvent(string eventJobId);

        /// <summary>
        /// Add a recuring event
        /// </summary>
        /// <param name="eventRequest"></param>
        /// <returns></returns>
        Event AddRecuringEvent(RecuringEventDto<T> eventRequest);

        /// <summary>
        /// Update a recuring event
        /// </summary>
        /// <param name="eventRequest"></param>
        /// <returns></returns>
        Event UpdateRecuringEvent(RecuringEventDto<T> eventRequest);


        /// <summary>
        /// Remove an event
        /// </summary>
        /// <param name="eventJobId"></param>
        void RemoveEvent(string eventJobId);

        /// <summary>
        /// Add a schedule event
        /// </summary>
        /// <param name="eventRequest"></param>
        /// <returns></returns>
        Event AddScheduleEvent(ScheduleEventDto<T> eventRequest);

        /// <summary>
        /// Update a schedule event
        /// </summary>
        /// <param name="eventRequest"></param>
        /// <returns></returns>
        Event UpdateScheduleEvent(ScheduleEventDto<T> eventRequest);

        string StartEventNow(T parameters);

        /// <summary>
        /// Start event
        /// </summary>
        /// <param name="eventJobId"></param>
        void StartEvent(string eventJobId);
    
        void StopEvent(string eventJobId);
        void AbortEvent(string eventJobId);
    }

    public class EventSchedulerService<T> : IEventSchedulerService<T>
    {
        private const int MinimumEventDuration = 30;
        private readonly ILogger<EventSchedulerService<T>> logger;
        private readonly IEventConsumer<T> eventConsumer;
        private readonly IEventBusService eventBusService;
        private readonly ScheduleContext scheduleContext;

        public EventSchedulerService(
            ILogger<EventSchedulerService<T>> logger,
            IEventConsumer<T> eventControler,
            IEventBusService eventBusService,
            ScheduleContext scheduleContext)
        {
            this.logger = logger;
            this.eventConsumer = eventControler;
            this.eventBusService = eventBusService;
            this.scheduleContext = scheduleContext;
        }


        public void StopAllMissedStop()
        {
            var missedEvents = scheduleContext.Events
            .Where(x => x.IsStarted)
            .AsEnumerable()
            .Where(missedJob => missedJob.LastExecution.Value.Add(missedJob.Duration) < DateTime.UtcNow)
            .ToArray();

            if (!missedEvents.Any())
                return;

            logger.LogWarning($"{missedEvents.Length} event are waiting to be stop");
            foreach (var missedJob in missedEvents)
            {
                AbortEvent(missedJob.EventId);
            }
        }

        public Event AddRecuringEvent(RecuringEventDto<T> eventRequest)
        {         

            RecuringPrecheck(eventRequest);
            var eventJob = EventJobFactory.CreateEntity(eventRequest);          
            RecurringJob.AddOrUpdate<IEventSchedulerService<T>>(eventJob.StartRecordJobId, x => x.StartEvent(eventJob.EventId), eventRequest.CronExpression);
            scheduleContext.Add(eventJob);
            scheduleContext.SaveChanges();
            return eventJob;
        }

        public Event UpdateRecuringEvent(RecuringEventDto<T> eventRequest)
        {
            RecuringPrecheck(eventRequest);

            var eventJob = GetEvent(eventRequest.Id);
            var needToReschedule = eventJob.CronExpression != eventRequest.CronExpression;
            EventJobFactory.UpdateEntity(eventJob, eventRequest);
            if (needToReschedule)
            {
                eventJob.CronExpression = eventRequest.CronExpression;
                RecurringJob.AddOrUpdate<IEventSchedulerService<T>>(eventJob.StartRecordJobId, x => x.StartEvent(eventJob.EventId), eventJob.CronExpression);
            }
            scheduleContext.Update(eventJob);
            scheduleContext.SaveChanges();
            return eventJob;
        }

        private static void RecuringPrecheck(RecuringEventDto<T> eventRequest)
        {
            if (eventRequest is null)
            {
                throw new ArgumentNullException(nameof(eventRequest));
            }

            if (string.IsNullOrWhiteSpace(eventRequest.CronExpression))
            {
                throw new ArgumentNullException(nameof(eventRequest.CronExpression));
            }

            if (eventRequest.DurationSeconds < MinimumEventDuration)
                throw new ArgumentOutOfRangeException(nameof(eventRequest.DurationSeconds), eventRequest.DurationSeconds, $"The duration must be superior to {MinimumEventDuration} seconds");
        }


        public Event UpdateScheduleEvent(ScheduleEventDto<T> eventRequest)
        {
            ShcedulePrecheck(eventRequest);

            var eventJob = GetEvent(eventRequest.Id);


            var needToRescheduleStart = eventJob.ScheduleAt != eventRequest.ScheduleAt;
            var needToRescheduleStop = eventJob.StopRecordJobId != null && eventJob.Duration.TotalSeconds != eventRequest.DurationSeconds;

            EventJobFactory.UpdateEntity(eventJob, eventRequest);
            if (needToRescheduleStart)
            {
                if (eventRequest.ScheduleAt < DateTime.UtcNow)
                    throw new ScheduleDateOutdatedException();

                logger.LogInformation($"Reschedule start for {eventJob.EventId}. Stop Job id: {eventJob.StartRecordJobId}");
                BackgroundJob.Delete(eventJob.StartRecordJobId);
                eventJob.StartRecordJobId = BackgroundJob.Schedule<IEventSchedulerService<T>>(x => x.StartEvent(eventJob.EventId), eventJob.ScheduleAt.Value);
            }

            if (needToRescheduleStop)
            {
                logger.LogInformation($"Reschedule stop for {eventJob.EventId}. Stop Job id: {eventJob.StopRecordJobId}");
                BackgroundJob.Delete(eventJob.StopRecordJobId);
                if (eventJob.ScheduleAt.Value.Add(eventJob.Duration) < DateTime.UtcNow)
                    eventJob.StopRecordJobId = BackgroundJob.Enqueue<IEventSchedulerService<T>>(x => x.StopEvent(eventJob.EventId));
                else
                    eventJob.StopRecordJobId = ScheduleStopRecord(eventJob);
            }

            scheduleContext.Update(eventJob);
            scheduleContext.SaveChanges();
            return eventJob;
        }


        public Event AddScheduleEvent(ScheduleEventDto<T> eventRequest)
        {
            ShcedulePrecheck(eventRequest);
            if (eventRequest.ScheduleAt < DateTime.UtcNow)
            {
                throw new ScheduleDateOutdatedException();
            }
            var eventJob = EventJobFactory.CreateEntity(eventRequest);

            var startJobId = BackgroundJob.Schedule<IEventSchedulerService<T>>(x => x.StartEvent(eventJob.EventId), eventRequest.ScheduleAt);
            eventJob.StartRecordJobId = startJobId;
            scheduleContext.Add(eventJob);
            scheduleContext.SaveChanges();
            return eventJob;
        }

        private static void ShcedulePrecheck(ScheduleEventDto<T> eventRequest)
        {
            if (eventRequest is null)
            {
                throw new ArgumentNullException(nameof(eventRequest));
            }

            if (eventRequest.ScheduleAt == DateTime.MinValue)
            {
                throw new ArgumentNullException(nameof(eventRequest.ScheduleAt));
            }

            if (eventRequest.DurationSeconds < MinimumEventDuration)
                throw new ArgumentOutOfRangeException(nameof(eventRequest.DurationSeconds), eventRequest.DurationSeconds, $"The duration must be superior to {MinimumEventDuration} seconds");

        }

        public void RemoveEvent(string eventJobId)
        {
            var eventJob = GetEvent(eventJobId);
            if (eventJob.IsStarted)
                AbortEvent(eventJobId);

            if (eventJob.ScheduleAt != null)
                BackgroundJob.Delete(eventJob.StartRecordJobId);
            else
                RecurringJob.RemoveIfExists(eventJob.StartRecordJobId);

            scheduleContext.Remove(eventJob);
            scheduleContext.SaveChanges();
        }

        public Event GetEvent(string eventJobId)
        {
            var eventJob = scheduleContext.Events.SingleOrDefault(x => x.EventId == eventJobId);
            if (eventJob == null)
                throw new NoEventFoundException(eventJobId);
            return eventJob;
        }


        public void StartEvent(string eventJobId)
        {
            var eventJob = GetEvent(eventJobId);
            try
            {
                var parameters = eventJob.ExtraParams.ToObject<T>();
                eventConsumer.Start(eventJob.EventId, eventJob.Duration, parameters);
                eventJob.StopRecordJobId = ScheduleStopRecord(eventJob);
                eventJob.IsStarted = true;

                eventBusService.NotifyForRecordStart<T>(eventJob.EventId,eventJob.Duration, parameters);

            }
            catch (Exception e)
            {
                eventJob.LastError = e.Message;
                throw;
            }
            finally
            {
                eventJob.LastExecution = DateTime.UtcNow;
                scheduleContext.Update(eventJob);
                scheduleContext.SaveChanges();
            }
        }

        public string StartEventNow(T parameters)
        {

            var eventJob = new Event
            {
                EventId = Guid.NewGuid().ToString(),
                Name = $"Manual event @{DateTime.UtcNow}",                
                ScheduleAt = DateTime.UtcNow,
                Duration = TimeSpan.FromDays(1),
                EventType = EventType.Manual,
                ExtraParams = JObject.FromObject( parameters)
                
            };
            scheduleContext.Events.Add(eventJob);
            scheduleContext.SaveChanges();

            var jobId = BackgroundJob.Enqueue<IEventSchedulerService<T>>(x => x.StartEvent(eventJob.EventId));
            eventJob.StartRecordJobId = jobId;
            scheduleContext.SaveChanges();

            return eventJob.EventId;
        }

        private static string ScheduleStopRecord(Event eventJob)
        {
            var eventJobId = eventJob.EventId;
            var stopTimestamp = eventJob.ScheduleAt == null ?
                DateTime.UtcNow.Add(eventJob.Duration) :
                eventJob.ScheduleAt.Value.Add(eventJob.Duration);

            var stopJobId = BackgroundJob.Schedule<IEventSchedulerService<T>>(x => x.StopEvent(eventJobId), stopTimestamp);
            return stopJobId;
        }


        public void StopEvent(string eventJobId)
        {
            var eventJob = GetEvent(eventJobId);
            if (eventJob == null)
                throw new NoEventFoundException(eventJobId);
            var parameter = eventJob.ExtraParams.ToObject<T>();
            eventConsumer.Stop(eventJob.EventId, parameter);
            BackgroundJob.Delete(eventJob.StopRecordJobId);

            eventJob.IsStarted = false;
            eventJob.StopRecordJobId = null;
            scheduleContext.Update(eventJob);
            scheduleContext.SaveChanges();

            eventBusService.NotifiyForRecordStop(
                eventJob.EventId,               
                  eventJob.Duration,
                  eventJob.ExtraParams);
        }


        public void AbortEvent(string eventJobId)
        {
            var eventJob = GetEvent(eventJobId);
            if (!eventJob.IsStarted)
                throw new EventNotStartedException();

            if (!string.IsNullOrEmpty(eventJob.StopRecordJobId))
                BackgroundJob.Delete(eventJob.StopRecordJobId);

            BackgroundJob.Enqueue<IEventSchedulerService<T>>(x => x.StopEvent(eventJobId));
        }

        public List<Event> GetEvents(string type, int start, int limit)
        {
            return scheduleContext.Events
            .Where(x => x.EventType == type)
            .Skip(start * 0).Take(limit)
            .ToList();
        }
    }
}