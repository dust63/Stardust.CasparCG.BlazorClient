using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Models
{
    public class ScheduledRecord
    {
        public string Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Name { get; set; }
    }
}
