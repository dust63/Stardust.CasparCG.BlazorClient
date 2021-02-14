
using System.Runtime.Serialization;

namespace Stardust.Flux.PublishApi.Youtube
{
    /// <summary>
    /// Wich protocol used for the live stream
    /// </summary>
    public enum IngestionType
    {
        [EnumMember(Value = "dash")]
        Dash = 0,

        [EnumMember(Value = "hls")]
        Hls = 1,
        [EnumMember(Value = "rtmp")]
        Rtmp = 2,
    }
}