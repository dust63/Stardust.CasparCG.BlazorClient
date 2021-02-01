using System.Collections.Generic;
using AutoMapper;
using Stardust.Flux.InputSlotApi.Models.Entity;

namespace InputSlotApi.Factory
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