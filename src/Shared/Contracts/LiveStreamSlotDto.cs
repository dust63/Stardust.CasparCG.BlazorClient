using System.Runtime.Serialization;

namespace Stardust.Flux.Contract
{
    [DataContract]
    public class LiveStreamSlotDto : OutputSlotDto
    {
         

        [DataMember]
        public string DefaultUrl { get; set; }

        [DataMember]

        public string OutputFormat { get; set; }

    }
}
