using System.Runtime.Serialization;

namespace ObsController.Controllers
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
