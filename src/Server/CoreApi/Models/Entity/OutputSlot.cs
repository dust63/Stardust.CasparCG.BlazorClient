using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.CoreApi.Models.Entity
{
    public class LiveStreamSlot : OutputSlot
    {

        public string DefaultUrl { get; set; }

        public string OutputFormat { get; set; }

    }

    public class RecordSlot : OutputSlot
    {               
        public string RecordParameters { get; set; }       

    }

    public abstract class OutputSlot
    {
        [Key]
        public int SlotId { get; set; }

        public int ServerId { get; set; }

        public int Channel { get; set; }

     
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual Server Server { get; set; }

        public string VideoCodec { get; set; }

        public string VideoEncodingOptions { get; set; }

        public string AudioCodec { get; set; }

        public string AudioEncodingOptions { get; set; }

        public string EncodingOptions { get; set; }

        
    }
}