using System.Runtime.Serialization;
using System;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.ScheduleEngine.Models
{
    [DataContract]
    [Serializable]
    public class Event
    {
        [Key]
        public string EventId { get; set; }

        public string RecordType { get; set; }

        [Required]
        public string Name { get; set; }


        public DateTime? ScheduleAt { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }
        public string CronExpression { get; set; }

        public DateTime? LastExecution { get; set; }

        public DateTime? NextExecution { get; set; }

        public string LastError { get; internal set; }

        public int RecordSlotId { get; set; }

        public string StartRecordJobId { get; set; }

        public string StopRecordJobId { get; set; }

        public bool IsStarted { get; set; }

        public object ExtraParams { get; set; }

    }

}