using System.Runtime.Serialization;
using System;
namespace Stardust.Flux.ScheduleEngine.DTO
{
    [DataContract]
    [Serializable]
    public class ScheduleRecordJobDto : BaseRecordJobDto
    {
        [DataMember]
        public DateTime ScheduleAt { get; set; }

    }

}