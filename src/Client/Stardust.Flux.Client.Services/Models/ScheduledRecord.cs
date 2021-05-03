using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Models
{
    public class ScheduledRecord
    {
        private ProgramMode mode;

        public ScheduledRecord()
        {
            YoutubeTags = new List<string>();
        }


        public string Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }


        [Required]
        public string Title { get; set; }


        public string Description { get; set; }


        [Required]
        public string Filename { get; set; }


        public List<string> YoutubeTags { get; set; }

        public bool IsPrivate { get; set; }


        public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;


        public ProgramMode Mode { get; set; }

        public string Logo { get; set; }

        public string OpenCreditMovie { get; set; }

        public string EndCreditMovie { get; set; }

        public enum ProgramMode
        {
            Normal = 0,
            Daily = 1,
            Weekly = 2
        }

        public ScheduledRecord Clone()
        {
            return new ScheduledRecord
            {
                Id = this.Id,
                StartTime = this.StartTime,
                Description = this.Description,
                DayOfWeek = this.DayOfWeek,
                EndTime = this.EndTime,
                Filename = this.Filename,
                IsPrivate = this.IsPrivate,
                Mode = this.Mode,
                Title = this.Title,
                YoutubeTags = this.YoutubeTags.ToList(),
                Logo = this.Logo,
                OpenCreditMovie = this.OpenCreditMovie,
                EndCreditMovie = this.EndCreditMovie
            };

        }


    }
}
