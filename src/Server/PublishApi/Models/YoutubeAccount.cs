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

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }


        public DateTime? IssuedUtc { get; internal set; }

        public ICollection<YoutubeUpload> YoutubeUploads { get; set; }
        public string RefreshToken { get; internal set; }
        public long? ExpiresInSeconds { get; internal set; }
        public string IdToken { get; internal set; }
        public string Scope { get; internal set; }
        public string TokenType { get; internal set; }
        public string AccessToken { get; internal set; }
    }
}