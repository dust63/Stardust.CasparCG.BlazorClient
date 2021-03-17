using System.Runtime.Serialization;
using System;
namespace Stardust.Flux.ScheduleEngine.DTO
{
    [DataContract]
    [Serializable]
    public class ScheduleEventDto : BaseEventDto
    {
        [DataMember]
        public DateTime ScheduleAt { get; set; }

    }

}