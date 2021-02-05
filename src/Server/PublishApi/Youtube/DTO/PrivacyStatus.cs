using System.Runtime.Serialization;

namespace Stardust.Flux.PublishApi.Youtube
{
    [DataContract]
    public enum PrivacyStatus
    {
        [EnumMember]
        Private = 0,
        [EnumMember]
        Public = 1,
        [EnumMember]
        Unlisted = 2

    }
}