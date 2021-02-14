using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;

namespace Stardust.Flux.PublishApi.Youtube
{
    [DataContract]
    public class BroadcastRequestDto
    {
        /// <summary>
        /// Account id to use for the broadcast
        /// </summary>
        [DataMember]
        public string AccountId { get; set; }
        /// <summary>
        /// The broadcast's title. Note that the broadcast represents exactly one YouTube video. You can set this field by modifying the broadcast resource or by setting the title field of the corresponding video resource.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The date and time that the broadcast is scheduled to start
        /// </summary>
        [DataMember]
        public DateTime ScheduleStartDate { get; set; }

        /// <summary>
        /// The date and time that the broadcast is scheduled to end. The value is specified in
        /// If a liveBroadcast resource does not specify a value for this property, then the broadcast is scheduled to continue indefinitely. Similarly, if you do not specify a value for this property, then YouTube treats the broadcast as if it will go on indefinitely.
        /// </summary>
        [DataMember]
        public DateTime ScheduleEndDate { get; set; }
        /// <summary>
        /// A map of thumbnail images associated with the broadcast. For each nested object in this object, the key is the name of the thumbnail image, and the value is an object that contains other information about the thumbnail.
        /// </summary>
        [DataMember]
        public ThumbnailDetails ThumbnailDetails { get; internal set; }

        /// <summary>
        /// The broadcast's description. As with the title, you can set this field by modifying the broadcast resource or by setting the description field of the corresponding video resource.
        /// </summary>
        [DataMember]
        public string Description { get; internal set; }

        /// <summary>
        /// The broadcast's privacy status. Note that the broadcast represents exactly one YouTube video, so the privacy settings are identical to those supported for videos. In addition, you can set this field by modifying the broadcast resource or by setting the privacyStatus field of the corresponding video resource.
        /// </summary>
        [DataMember]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PrivacyStatus PrivacyStatus { get; set; }

        /// <summary>
        ///  This property allows the channel owner to designate the broadcast as being child-directed
        /// </summary>
        [DataMember]
        public bool? SelfDeclaredMadeForKids { get; internal set; }

        /// <summary>
        /// Indicates whether this broadcast should start automatically when you start streaming video on the bound live stream.
        /// </summary>
        [DataMember]
        public bool? EnableAutoStart { get; internal set; }

        /// <summary>
        /// Indicates whether this broadcast should stop automatically around one minute after the channel owner stops streaming video on the bound video stream.
        /// </summary>
        [DataMember]
        public bool? EnableAutoStop { get; internal set; }


        [DataMember]
        public bool? EnableClosedCaptions { get; internal set; }

        /// <summary>
        /// This setting indicates whether YouTube should enable content encryption for the broadcast.
        ///When you update a broadcast, this property must be set if your API request includes the contentDetails part in the part parameter value. However, when you insert a broadcast, the property is optional and has a default value of false.
        /// </summary>
        [DataMember]
        public bool? EnableContentEncryption { get; internal set; }


        /// <summary>
        /// When you update a broadcast, this property must be set if your API request includes the contentDetails part in the part parameter value. However, when you insert a broadcast, the property is optional and has a default value of true.
        /// </summary>
        [DataMember]
        public bool? EnableDvr { get; internal set; }

        /// <summary>
        /// This setting indicates whether the broadcast video can be played in an embedded player. If you choose to archive the video (using the enableArchive property), this setting will also apply to the archived video
        /// </summary>
        [DataMember]
        public bool? EnableEmbed { get; internal set; }

        /// <summary>
        /// This setting indicates whether YouTube will automatically start recording the broadcast after the event's status changes to live.

        ///This property's default value is true, and it can only be set to false if the broadcasting channel is allowed to disable recordings for live broadcasts.
        /// </summary>
        [DataMember]
        public bool? RecordFromStart { get; internal set; }


        /// <summary>
        /// This value determines whether the monitor stream is enabled for the broadcast. 
        /// If the monitor stream is enabled, then YouTube will broadcast the event content on a special stream intended only for the broadcaster's consumption. The broadcaster can use the stream to review the event content and also to identify the optimal times to insert cuepoints.
        /// </summary>
        [DataMember]
        public bool? EnableMonitorStream { get; internal set; }

        /// <summary>
        /// unsigned integer 
        /// If you have set the enableMonitorStream property to true, then this property determines the length of the live broadcast delay.
        /// When you update a broadcast, this property must be set if your API request includes the contentDetails part in the part parameter value
        /// </summary>
        [DataMember]
        public long? BroadcastStreamDelayMs { get; internal set; }
    }
}