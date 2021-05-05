using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Models
{
    public class OutputSlot
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string VideoCodec { get; set; }

        public string AudioCodec { get; set; }

        public int VideoBitRate { get; set; }

        
    }
}
