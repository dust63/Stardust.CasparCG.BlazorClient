using System.Runtime.Serialization;

namespace Stardust.Flux.ObsController.Dto
{
    [DataContract]
    public class RecordStatus
    {
        [DataMember]
        public bool IsRecording { get; set; }

        [DataMember]
        public string RecordElapsed { get; set; }
        public string RecordFileName { get; internal set; }
    }
}
