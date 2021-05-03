using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.CoreApi
{
    [Serializable]
    internal class ServerNotFoundException : Exception
    {
        private int serverId;

        public ServerNotFoundException()
        {
        }

        public ServerNotFoundException(int serverId):base($"The server with id {serverId} was not found")
        {
            this.serverId = serverId;
        }

   
        public ServerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}