using System.Collections.Generic;

namespace Stardust.Flux.CoreApi.Models.Entity
{

    public class Slot
    {

        public Slot()
        {
            AdditionalsData = new HashSet<AdditionalSlotData>();
        }

        public int SlotId { get; set; }

        public int ServerId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public ICollection<AdditionalSlotData> AdditionalsData { get; set; }

        public virtual Server Server { get; set; }
    }
}