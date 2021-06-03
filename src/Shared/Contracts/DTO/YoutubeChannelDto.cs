using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract.DTO
{

    [DataContract]
    public class YoutubeChannelDto
    {

        //
        // Summary:
        //     The country of the channel.
        [DataMember(Name = "country")]
        public string Country { get; set; }
        //
        // Summary:
        //     The custom url of the channel.
        [DataMember(Name = "customUrl")]
        public string CustomUrl { get; set; }
        //
        // Summary:
        //     The language of the channel's default title and description.
        [DataMember(Name = "defaultLanguage")]
        public string DefaultLanguage { get; set; }
        //
        // Summary:
        //     The description of the channel.
        [DataMember(Name = "description")]
        public string Description { get; set; }

        //
        // Summary:
        //     The date and time that the channel was created.
        [DataMember(Name = "publishedAt")]
        public string PublishedAtRaw { get; set; }


        [DataMember(Name = "thumbnails")]
        public string Thumbnails { get; set; }
        //
        // Summary:
        //     The channel's title.
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "subscriberCount")]
        public ulong? SubscriberCount { get; set; }

        [DataMember(Name = "viewCount")]
        public ulong? ViewCount { get; set; }
    }
}

