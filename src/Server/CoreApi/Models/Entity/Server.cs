using System.Collections.Generic;

namespace Stardust.Flux.CoreApi.Models.Entity
{
    public class Server
    {

        public Server()
        {
            Slots = new HashSet<Slot>();
        }

        public int ServerId { get; set; }

        public string Hostname { get; set; }

        public int Port { get; set; }


        public string Name { get; set; }


        public ICollection<Slot> Slots { get; set; }
    }
}