using Stardust.Flux.Crosscutting.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.Client.Models
{
    public class InputSlot
    {
        public int Id{ get; set; }

        [Required]
        public string InputType{ get; set; }

        [RequiredForAny(Values =new[] { "Decklink"}, PropertyName = nameof(InputType))]
        [Range(1,10)]
        public ushort DecklinkId{ get; set; }

        [RequiredForAny(Values = new[] { "NDI" }, PropertyName = nameof(InputType))]
        public string NdiUrl{ get; set; }

        [RequiredForAny(Values = new[] { "Decklink" }, PropertyName = nameof(InputType))]
        public string InputVideoFormat{ get; set; }

        public string Description{ get; set; }
    }
}