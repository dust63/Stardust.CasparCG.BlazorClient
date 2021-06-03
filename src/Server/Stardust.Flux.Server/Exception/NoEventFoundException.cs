using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.ScheduleEngine
{
    [Serializable]
    public class NoEventFoundException : Exception
    {
        public NoEventFoundException(string id) : base($"No record job found with {id}")
        {
        }



        public NoEventFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoEventFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}