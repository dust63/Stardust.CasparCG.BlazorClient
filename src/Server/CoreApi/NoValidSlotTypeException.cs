using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.CoreApi.Controllers
{
    [Serializable]
    internal class NoValidSlotTypeException : Exception
    {
    
        public NoValidSlotTypeException(string actualSlotType, string desiredSlotType) : base($"The actual slot type {actualSlotType} is not correct for {desiredSlotType}")
        {
        }

        public NoValidSlotTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoValidSlotTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}