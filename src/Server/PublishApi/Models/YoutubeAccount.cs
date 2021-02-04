using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.PublishApi.Models
{
    public class YoutubeAccount
    {

        [Key]
        public string Key { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }
    }
}