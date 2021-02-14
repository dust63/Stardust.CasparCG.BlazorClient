namespace Stardust.Flux.PublishApi.Youtube
{
    public class YoutubeApiOptions
    {

        public const string SectionName = "YoutubeApi";

        public string ClientId { get; set; }

        public string ClientSecrets { get; set; }
        public string RedirectUrl { get; set; }
    }
}