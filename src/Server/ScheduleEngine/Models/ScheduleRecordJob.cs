using System.Runtime.Serialization;
using System;
namespace ScheduleEngine.Models
{
    [DataContract]
    [Serializable]
    public class ScheduleRecordJob
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public DateTime? ScheduleAt { get; set; }
    }

}