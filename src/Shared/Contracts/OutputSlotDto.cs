using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace Stardust.Flux.Contract
{


    [DataContract]
    public abstract class OutputSlotDto
    {
           
        [DataMember]
        public int SlotId { get; set; }


        [DataMember]
        public int ServerId { get; set; }

        [DataMember]
        public int Channel { get; set; }
        

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
              

        [DataMember]
        public ServerDto Server { get; set; }

        [DataMember]
        public string VideoCodec { get; set; }

        [DataMember]
        public string VideoEncodingOptions { get; set; }

        [DataMember]
        public string AudioCodec { get; set; }

        [DataMember]
        public string AudioEncodingOptions { get; set; }

        [DataMember]
        public string EncodingOptions { get; set; }
          




        public override string ToString()
        {
            return
                $"{nameof(SlotId)}: {SlotId}";            
               
        }


    }
}
