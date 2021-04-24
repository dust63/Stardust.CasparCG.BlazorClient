using System.Runtime.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Stardust.Flux.ScheduleEngine.Models
{
    [DataContract]
    [Serializable]
    public class Event
    {
        [Key]
        public string EventId { get; set; }

        public string EventType { get; set; }

        public string ParamType { get; set; }

        [Required]
        public string Name { get; set; }


        public DateTime? ScheduleAt { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }
        public string CronExpression { get; set; }

        public DateTime? LastExecution { get; set; }

        public DateTime? NextExecution { get; set; }

        public string LastError { get; internal set; }

     

        public string StartRecordJobId { get; set; }

        public string StopRecordJobId { get; set; }

        public bool IsStarted { get; set; }

        public JObject ExtraParams { get; set; }

    }

}