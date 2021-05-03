using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract
{
    [DataContract]
    public class MovieDto
    {


        [DataMember]
        public long Frames { get; set; }
        [DataMember]
        public decimal Fps { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public long Size { get; set; }
        [DataMember]
        public DateTime LastUpdated { get; set; }

        public override string ToString()
        {
            return FullName;
        }

    }
}
