using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract
{
    [DataContract]
    public class TemplateDto
    {
        [DataMember]
        public string Folder { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }

        [DataMember]
        public string FullName { get; set; }
    }
}
