using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Stardust.Flux.Contract;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Factory
{
    public class SlotProfile : Profile
    {
        public SlotProfile()
        {
            CreateMap<OutputSlot, OutputSlotDto>()
            .ReverseMap();


            CreateMap<RecordSlot, RecordSlotDto>()
           .ReverseMap();

            CreateMap<LiveStreamSlot, LiveStreamSlotDto>()
           .ReverseMap();
        }
    }   
}