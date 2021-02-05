using System;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Flux.PublishApi.Models
{
    public class YoutubeUpload
    {
        public YoutubeUpload()
        {
            YoutubeUploadId = Guid.NewGuid().ToString();
            CreatedOn = DateTime.UtcNow;
        }
        [Key]
        public string YoutubeUploadId { get; set; }

        public YoutubeAccount YoutubeAccount { get; set; }

        public string YoutubeAccountId { get; set; }

        public string VideoId { get; set; }

        public string State { get; set; }

        public long BytesSent { get; set; }
        public string Error { get; set; }

        public string FilePath { get; set; }

        /// <summary>
        /// Utc created date
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}