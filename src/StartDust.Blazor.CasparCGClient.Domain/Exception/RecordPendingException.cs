using System;
using System.Runtime.Serialization;

namespace StartDust.Blazor.CasparCGClient.Domain
{
    [Serializable]
    public class RecordPendingException : Exception
    {
        public RecordPendingException() : base("A record is in course")
        {
        }

       

        public RecordPendingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecordPendingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}