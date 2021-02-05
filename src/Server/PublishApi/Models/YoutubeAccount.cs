using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.PublishApi.Models
{
    public class YoutubeAccount
    {
        public YoutubeAccount()
        {
            YoutubeUploads = new HashSet<YoutubeUpload>();
            CreatedOn = DateTime.UtcNow;
            ModifiedOn = DateTime.UtcNow;
        }

        [Key]
        public string Key { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }


        public ICollection<YoutubeUpload> YoutubeUploads { get; set; }
    }
}