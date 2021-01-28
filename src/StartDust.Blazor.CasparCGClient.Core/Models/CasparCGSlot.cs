using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace StartDust.Blazor.CasparCGClient.Core
{
    /// <summary>
    /// A caspar CG record slot
    /// </summary>
    [Serializable]
    [DataContract]
    public class CasparCGSlot : IEquatable<CasparCGSlot>
    {
        public CasparCGSlot()
        {
            ID = Guid.NewGuid().ToString();
        }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Channel { get; set; }

        [DataMember]
        public InputSource? InputSource { get; set; }

        [DataMember]
        public string Hostname { get; set; }

        [DataMember]
        public string CodecOptions { get; set; } = "-codec:v h264_nvenc -profile:v high444p -pixel_format:v yuv444p -preset:v default";

        public override string ToString()
        {
            return string.Join(",",Name, Hostname, Channel, InputSource);
        }
        public bool Equals(CasparCGSlot other)
        {
            if (other == null)
                return false;

          return  
                other.Channel == Channel &&
                other.InputSource == InputSource &&
                string.Equals(other.Hostname, Hostname, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CasparCGSlot);
        }
    }

    [Serializable]
    [DataContract]
    public enum InputSource
    {
        [EnumMember]
        Decklink,
        [EnumMember]
        NDI,
    }
}
