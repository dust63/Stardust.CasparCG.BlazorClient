using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.ScheduleEngine
{
    [Serializable]
    public class NotRecordingException : Exception
    {
        public NotRecordingException() : base("Can't abort a record that are not recording")
        {

        }


        protected NotRecordingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}