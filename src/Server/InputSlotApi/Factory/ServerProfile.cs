using System.Linq;
using AutoMapper;
using Stardust.Flux.Contract;
using Stardust.Flux.InputSlotApi.Models.Entity;

namespace Stardust.Flux.InputSlotApi.Factory
{
    public class ServerProfile : Profile
    {
        public ServerProfile()
        {
            CreateMap<Server, ServerDto>()
            .ReverseMap()
            .ForMember(src => src.ServerId, opt => opt.Ignore());
        }
    }
}