using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract.DTO.Schedule
{

    [DataContract]
    public class ScheduleEventResponse<TParam> : ScheduleEventDto<TParam>, IBaseEventResponseDto
    {
        public ScheduleEventResponse(
            string startRecordJobId, 
            string stopRecordJobId,
            ScheduleEventDto<TParam> scheduleEvent)
        {
            StartRecordJobId = startRecordJobId;
            StopRecordJobId = stopRecordJobId;
            ScheduleAt = scheduleEvent.ScheduleAt;
            DurationSeconds = scheduleEvent.DurationSeconds;
            Name = scheduleEvent.Name;
            Id = scheduleEvent.Id;
            ExtraParams = scheduleEvent.ExtraParams;          
        }


        [DataMember]
        public string StartRecordJobId { get; set; }


        [DataMember]
        public string StopRecordJobId { get; set; }
    }
}
