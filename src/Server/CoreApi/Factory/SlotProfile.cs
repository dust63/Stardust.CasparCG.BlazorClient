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
            CreateMap<Slot, SlotDto>()
            .ForMember(src => src.AdditionalsData, opt => opt.MapFrom(src => src.AdditionalsData.ToDictionary(x => x.Key, x => x.Value)))
            .ReverseMap()
            .ForMember(src => src.AdditionalsData, opt => opt.Ignore());
        }
    }
}