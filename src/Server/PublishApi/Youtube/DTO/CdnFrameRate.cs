
using System.Runtime.Serialization;

namespace Stardust.Flux.PublishApi.Youtube
{
    /// <summary>
    /// Framerate for the live stream sent to the CDN of Youtube
    /// </summary>
    public enum CdnFrameRate
    {
        [EnumMember(Value = "variable")]
        Variable = 0,

        [EnumMember(Value = "30fps")]
        _30fps = 1,

        [EnumMember(Value = "60fps")]
        _60fps = 2,


    }
}