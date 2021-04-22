using System.Runtime.Serialization;
using System;

namespace Stardust.Flux.Contract.DTO.Schedule
{
    [Serializable]
    public abstract class BaseEventDto
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }



        [DataMember]
        public double DurationSeconds { get; set; }


        public override string ToString()
        {
            return string.Join("-", Id, Name);
        }

    }

}