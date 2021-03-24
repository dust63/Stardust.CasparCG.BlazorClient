using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.CasparControler
{
    [Serializable]
    internal class NoChannelFoundException : Exception
    {
        private int channelId;

        public NoChannelFoundException()
        {
        }

        public NoChannelFoundException(int channelId):base($"Can't find any CasparCg channel with id: {channelId}")
        {
            this.channelId = channelId;
        }

   
        public NoChannelFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoChannelFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}