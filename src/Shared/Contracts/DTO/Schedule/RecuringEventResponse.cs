using System.Runtime.Serialization;

namespace Stardust.Flux.Contract.DTO.Schedule
{
    [DataContract]
    public class RecuringEventResponse<TParam> : RecuringEventDto<TParam>, IBaseEventResponseDto
    {
        public RecuringEventResponse(
            string startRecordJobId,
            string stopRecordJobId,
            RecuringEventDto<TParam> recuringEvent)
        {
            StartRecordJobId = startRecordJobId;
            StopRecordJobId = stopRecordJobId;
            CronExpression = recuringEvent.CronExpression;
            DurationSeconds = recuringEvent.DurationSeconds;
            Name = recuringEvent.Name;
            Id = recuringEvent.Id;
            ExtraParams = recuringEvent.ExtraParams;
        }


        [DataMember]
        public string StartRecordJobId { get; set; }


        [DataMember]
        public string StopRecordJobId { get; set; }
    }
}
