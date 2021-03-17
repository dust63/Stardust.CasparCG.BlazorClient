using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.ScheduleEngine
{
    [Serializable]
    public class NotEventStartedException : Exception
    {
        public NotEventStartedException() : base("Can't abort a event that are not started")
        {

        }


        protected NotEventStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}