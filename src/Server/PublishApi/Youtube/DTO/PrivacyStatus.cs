using System.Runtime.Serialization;

namespace Stardust.Flux.PublishApi.Youtube
{
    [DataContract]
    public enum PrivacyStatus
    {
        [EnumMember(Value = "private")]
        Private = 0,
        [EnumMember(Value = "public")]
        Public = 1,
        [EnumMember(Value = "unlisted")]
        Unlisted = 2

    }
}