using System.Runtime.Serialization;
using System;
namespace Stardust.Flux.ScheduleEngine.DTO
{
    [DataContract]
    [Serializable]
    public class ScheduleEventDto<TParam> : BaseEventDto
    {
        [DataMember]
        public DateTime ScheduleAt { get; set; }

        public new TParam ExtraParams { get; set; }

    }

}