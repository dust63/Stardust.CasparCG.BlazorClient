using System.Collections.Generic;
using AutoMapper;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Factory
{
    public class AdditionalDataProfile : Profile
    {
        public AdditionalDataProfile()
        {
            CreateMap<KeyValuePair<string, string>, AdditionalSlotData>()
            .ConvertUsing(src => new AdditionalSlotData { Key = src.Key, Value = src.Value });
        }
    }
}