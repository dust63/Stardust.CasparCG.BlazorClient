namespace Stardust.Flux.ScheduleEngine.DTO
{
    public class RecordResponseDto<T> where T : BaseRecordJobDto
    {

        public RecordResponseDto(string startRecordJobId, string stopRecordJobId, T record)
        {
            StartRecordJobId = startRecordJobId;
            StopRecordJobId = stopRecordJobId;
            Record = record;
        }

        public RecordResponseDto()
        {

        }

        public string StartRecordJobId { get; set; }

        public string StopRecordJobId { get; set; }

        public T Record { get; set; }
    }
}