using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.CasparControler
{
    [Serializable]
    internal class CanFindAdditionalDataException : Exception
    {
        

        public CanFindAdditionalDataException(string key) : base($"Can't find additional date with key {key}")
        {
        }

        public CanFindAdditionalDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CanFindAdditionalDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}