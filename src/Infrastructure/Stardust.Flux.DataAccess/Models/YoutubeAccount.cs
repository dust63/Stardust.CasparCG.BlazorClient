using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.DataAccess.Models
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


        public DateTime? IssuedUtc { get; set; }

        public ICollection<YoutubeUpload> YoutubeUploads { get; set; }
        public string RefreshToken { get; set; }
        public long? ExpiresInSeconds { get; set; }
        public string IdToken { get; set; }
        public string Scope { get; set; }
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
    }
}