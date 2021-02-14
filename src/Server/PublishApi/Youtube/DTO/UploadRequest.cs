using System.Data;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Stardust.Flux.PublishApi.Youtube
{
    [DataContract]
    public class UploadRequest
    {

        public UploadRequest(string accountId, string filePath, string title, string categoryId)
        {
            FilePath = filePath;
            Title = title;
            CategoryId = categoryId;
            AccountId = accountId;
        }

        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string[] Tags { get; set; }
        [DataMember]
        public string CategoryId { get; set; }

        [DataMember]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PrivacyStatus PrivacyStatus { get; set; }

        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public static bool? NotifySubscribers { get; set; }


        public override string ToString()
        {
            return string.Join("-", AccountId, Title, FilePath);
        }

    }
}