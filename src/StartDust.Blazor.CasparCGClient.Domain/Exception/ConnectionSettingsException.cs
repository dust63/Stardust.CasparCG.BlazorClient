using System;
using System.Runtime.Serialization;

namespace StartDust.Blazor.CasparCGClient.Domain
{
    [Serializable]
    public class ConnectionSettingsException : Exception
    {
        public ConnectionSettingsException() : base("Can't connect to CasparCG Server")
        {
        }



        public ConnectionSettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConnectionSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}