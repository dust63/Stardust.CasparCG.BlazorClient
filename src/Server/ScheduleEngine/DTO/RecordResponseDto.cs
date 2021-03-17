namespace Stardust.Flux.ScheduleEngine.DTO
{
    public class EventResponseDto<T> where T : BaseEventDto
    {

        public EventResponseDto(string startRecordJobId, string stopRecordJobId, T record)
        {
            StartRecordJobId = startRecordJobId;
            StopRecordJobId = stopRecordJobId;
            Record = record;
        }

        public EventResponseDto()
        {

        }

        public string StartRecordJobId { get; set; }

        public string StopRecordJobId { get; set; }

        public T Record { get; set; }
    }
}