using System.Collections.Generic;
using Stardust.Flux.Contract.DTO.Schedule;
using Stardust.Flux.DataAccess.Models;

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
}