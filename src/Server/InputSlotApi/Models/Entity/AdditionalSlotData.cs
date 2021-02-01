namespace Stardust.Flux.InputSlotApi.Models.Entity
{
    public class AdditionalSlotData
    {
        public int Id { get; set; }

        public int SlotId { get; set; }
        public Slot Slot { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

    }
}