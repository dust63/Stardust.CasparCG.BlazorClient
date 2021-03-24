using System;
using System.Collections.Generic;
using Stardust.Flux.Contract;
using Stardust.Flux.Crosscutting.Extensions;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Factory
{
    public static class SlotFactory
    {
        public static void UpdateValueFrom(this RecordSlot slot, RecordSlotDto slotDto)
        {
            slot.Name = slotDto.Name;           
            slot.VideoCodec = slotDto.VideoCodec;
            slot.AudioCodec = slotDto.AudioCodec;
            slot.VideoEncodingOptions = slotDto.VideoEncodingOptions;
            slot.AudioEncodingOptions = slotDto.AudioEncodingOptions;
            slot.Channel = slotDto.Channel;
            slot.Description = slotDto.Description;
            slot.EncodingOptions = slotDto.EncodingOptions;
            slot.RecordParameters = slotDto.RecordParameters;
        }

        public static void UpdateValueFrom(this LiveStreamSlot slot, LiveStreamSlotDto slotDto)
        {
            slot.Name = slotDto.Name;
            slot.DefaultUrl = slotDto.DefaultUrl;
            slot.VideoCodec = slotDto.VideoCodec;
            slot.AudioCodec = slotDto.AudioCodec;
            slot.VideoEncodingOptions = slotDto.VideoEncodingOptions;
            slot.AudioEncodingOptions = slotDto.AudioEncodingOptions;
            slot.Channel = slotDto.Channel;
            slot.Description = slotDto.Description;
            slot.EncodingOptions = slotDto.EncodingOptions;        
        }


    }
}