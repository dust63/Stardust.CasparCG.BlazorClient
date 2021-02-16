using System.Runtime.Serialization;
using System;
namespace Stardust.Flux.ScheduleEngine.DTO
{
    [Serializable]
    public abstract class BaseRecordJobDto
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Filename { get; set; }

        [DataMember]
        public double DurationSeconds { get; set; }

        [DataMember]
        public int RecordSlotId { get; set; }

        public override string ToString()
        {
            return string.Join("-", Id, Name, RecordSlotId, Filename);
        }

    }

}