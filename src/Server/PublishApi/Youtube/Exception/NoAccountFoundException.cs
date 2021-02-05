using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.PublishApi.Youtube
{
    public class NoAccountFoundException : Exception
    {

        public NoAccountFoundException(string accountId) : base($"No account found for {accountId}")
        {
        }

        public NoAccountFoundException(string accountId, Exception innerException) : base($"No account found for {accountId}", innerException)
        {
        }

        protected NoAccountFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}