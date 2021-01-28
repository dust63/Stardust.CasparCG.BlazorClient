using System.Runtime.Serialization;
using System;
namespace ScheduleEngine.Models
{
    [DataContract]
    [Serializable]
    public class RecuringRecordJob
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string CronExpression { get; set; }

        [DataMember]
        public DateTime? LastExecution { get; set; }

        [DataMember]
        public DateTime? NextExecution { get; set; }
        public string LastError { get; internal set; }
    }

}