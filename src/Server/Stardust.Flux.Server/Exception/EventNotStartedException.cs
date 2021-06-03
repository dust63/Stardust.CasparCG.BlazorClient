using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.ScheduleEngine
{
    [Serializable]
    public class EventNotStartedException : Exception
    {
        public EventNotStartedException() : base("Can't abort a event that are not started")
        {

        }


        protected EventNotStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}