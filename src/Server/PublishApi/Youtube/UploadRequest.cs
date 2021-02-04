namespace Stardust.Flux.PublishApi.Youtube
{
    public class UploadRequest
    {
        public string ChannelId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string[] Tags { get; set; }

        public string CategoryId { get; set; }

        public string PrivacyStatus { get; set; }

        public string FilePath { get; set; }

    }
}