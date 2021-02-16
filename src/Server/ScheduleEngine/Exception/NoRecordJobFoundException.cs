using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.ScheduleEngine
{
    [Serializable]
    public class NoRecordJobFoundException : Exception
    {
        public NoRecordJobFoundException(string id) : base($"No record job found with {id}")
        {
        }



        public NoRecordJobFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoRecordJobFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}