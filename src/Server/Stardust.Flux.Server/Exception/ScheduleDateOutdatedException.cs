using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.ScheduleEngine
{
    [Serializable]
    public class ScheduleDateOutdatedException : Exception
    {
        public ScheduleDateOutdatedException() : base("The schedule date is outdated")
        {

        }
        protected ScheduleDateOutdatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}