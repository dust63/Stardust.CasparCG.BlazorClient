using System.Runtime.Serialization;

namespace Stardust.Flux.Contract
{
    [DataContract]
    public class ServerDto
    {
        [DataMember]
        public int ServerId { get; set; }

        [DataMember]
        public string Hostname { get; set; }

        [DataMember]
        public int Port { get; set; }


        [DataMember]
        public string Name { get; set; }


        public override string ToString()
        {
            return string.Join(",",
                $"{nameof(ServerId)}: {ServerId}",
                $"{nameof(Name)}: {Name}",
                $"{nameof(Hostname)}: {Hostname}", 
                $"{nameof(Port)}: {Port}");
        }
    }
}