
using System.Runtime.Serialization;

namespace Stardust.Flux.PublishApi.Youtube
{

    /// <summary>
    /// Video resolution for the live stream sent to the CDN of Youtube
    /// </summary>
    public enum CdnResolution
    {
        [EnumMember(Value = "240p")]
        _240p = 0,

        [EnumMember(Value = "360p")]
        _360p = 1,

        [EnumMember(Value = "480p")]
        _480p = 2,

        [EnumMember(Value = "720p")]
        _720p = 3,

        [EnumMember(Value = "1080p")]
        _1080p = 4,

        [EnumMember(Value = "1440p")]
        _1440p = 5,

        [EnumMember(Value = "2160p")]
        _2160p = 6,
    }
}