using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Models
{
    public class ManualRecord
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime? Duration { get; set; }

        public string Filename { get; set; }
    }
}
