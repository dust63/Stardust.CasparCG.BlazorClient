using System;
using System.Collections.Generic;
using Stardust.Flux.Contract;
using Stardust.Flux.Crosscutting.Extensions;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Factory
{
    public static class SlotFactory
    {
        public static void UpdateValueFrom(this Slot slot, SlotDto slotDto)
        {
            slot.Name = slotDto.Name;
            slot.Type = slotDto.Type;

            var addtionalsData = slotDto?.AdditionalsData ?? new Dictionary<string, string>();
            slot.AdditionalsData.MergeChildren(
                addtionalsData, (src, dst) => string.Equals(src.Key, dst.Key, StringComparison.OrdinalIgnoreCase),
                (src) => new AdditionalSlotData { Key = src.Key, Value = src.Value },
                (src, dst) => dst.Value = src.Value);
        }
    }
}