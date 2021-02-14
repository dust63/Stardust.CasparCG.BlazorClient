
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Stardust.Flux.PublishApi.Youtube
{
    /// <summary>
    /// Data need to create a live stream on youtube
    /// </summary>
    [DataContract]
    public class LiveStreamRequestDto
    {
        public LiveStreamRequestDto()
        {
            IsReusable = true;
            IngestionType = IngestionType.Rtmp;
            CdnFrameRate = CdnFrameRate.Variable;
            CdnResolution = CdnResolution._720p;
        }

        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string LiveStreamId { get; set; }

        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IngestionType IngestionType { get; set; }

        [DataMember]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CdnResolution CdnResolution { get; set; }

        [DataMember]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CdnFrameRate CdnFrameRate { get; set; }

        [DataMember]
        public bool IsReusable { get; set; }

    }
}