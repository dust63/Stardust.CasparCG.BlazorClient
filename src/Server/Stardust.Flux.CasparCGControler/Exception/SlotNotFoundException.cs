using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.CasparControler
{
    [Serializable]
    internal class SlotNotFoundException : Exception
    {
        private int slotId;

        public SlotNotFoundException()
        {
        }

        public SlotNotFoundException(int slotId) : base($"No slot found with Id: {slotId}")
        {
            this.slotId = slotId;
        }

      

        public SlotNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SlotNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}