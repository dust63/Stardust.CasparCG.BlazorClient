using System.Runtime.Serialization;

namespace Stardust.Flux.Contract
{


    [DataContract]
    public class RecordSlotDto : OutputSlotDto
    {            
            
        [DataMember]
        public string RecordParameters { get; set; }

        public override string ToString()
        {
            return string.Join(",",
                base.ToString(),                      
                $"{nameof(RecordParameters)}: {RecordParameters}");      
        }

    }
}
