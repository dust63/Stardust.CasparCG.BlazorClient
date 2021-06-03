using System;

namespace Stardust.Flux.ScheduleEngine.Services
{
    /// <summary>
    /// Service in charge to notify bus when event is started or stopped
    /// </summary>
    public interface IEventNotificationService
    {

        void NotifyForRecordStart<T>(string jobId, TimeSpan duration, T parameters);


        void NotifiyForRecordStop<T>(string jobId, TimeSpan duration, T parameters);

    }

    /// <summary>
    /// Service in charge to notify bus when event is started or stopped
    /// </summary>
    public class EventNotificationService : IEventNotificationService
    {
   

        public EventNotificationService()
        {
           
        }


        public void NotifiyForRecordStop<T>(string jobId,  TimeSpan duration, T parameters)
        {
          //TODO SIGNAL RECORDON SIGNAL R
        }

        public void NotifyForRecordStart<T>(string jobId, TimeSpan duration, T parameters)
        {
            //TODO SIGNAL RECORDON SIGNAL R
        }
    }
}