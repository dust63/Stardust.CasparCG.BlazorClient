using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Models
{
    public class ScheduledRecord
    {
        public ScheduledRecord()
        {
            YoutubeTags = new List<string>();
        }

        public bool IsRecuring { get; set; }

        public string Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
          

        [Required]
        public string Title { get; set; }


        public string Description { get; set;}


        public List<string> YoutubeTags { get; set; }

        public bool IsPrivate { get; set; }


        public string RecurrenceRule { get; set; }


    }
}
