using System.Runtime.Serialization;
using System;
namespace Stardust.Flux.ScheduleEngine.DTO
{
    [DataContract]
    [Serializable]
    public class RecuringEventDto<TParam> : BaseEventDto
    {

        [DataMember]
        public string CronExpression { get; set; }

        [DataMember]
        public DateTime? LastExecution { get; set; }

        [DataMember]
        public DateTime? NextExecution { get; set; }
        public string LastError { get; internal set; }

        public new TParam ExtraParams { get; set; }
    }



}