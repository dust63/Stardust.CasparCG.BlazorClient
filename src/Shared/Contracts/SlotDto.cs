using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.Contract
{
    [DataContract]
    public class SlotDto
    {

        public SlotDto()
        {
            AdditionalsData = new Dictionary<string, string>();
        }

        [DataMember]
        public int SlotId { get; set; }


        [DataMember]
        public int ServerId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public Dictionary<string, string> AdditionalsData { get; set; }
    }
}
