using Stardust.Flux.Contract.DTO;
using System.Collections.Generic;

namespace Stardust.Flux.Client.Services
{
    public interface IEncodingOptionProvider
    {
        IList<EncodingOptionDto> GetAll();
    }
}