using System.Runtime.Serialization;
using System;

namespace Stardust.Flux.Contract.DTO.Schedule
{
    [DataContract]
    [Serializable]
    public class ScheduleEventDto<TParam> : BaseEventDto
    {
        [DataMember]
        public DateTime ScheduleAt { get; set; }

        [DataMember]
        public TParam ExtraParams { get; set; }   
    }

}