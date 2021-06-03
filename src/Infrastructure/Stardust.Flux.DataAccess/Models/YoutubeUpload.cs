using System;
using System.ComponentModel.DataAnnotations;


namespace Stardust.Flux.DataAccess.Models
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

        [Required]
        public string YoutubeAccountId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        public string CategoryId { get; set; }

        [Required]
        public string PrivacyStatus { get; set; }

        [Required]
        public string FilePath { get; set; }

        public static bool? NotifySubscribers { get; set; }

        public string YoutubeVideoId { get; set; }
        public string State { get; set; }
        public long BytesSent { get; set; }
        public string Error { get; set; }


        /// <summary>
        /// Utc created date
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}