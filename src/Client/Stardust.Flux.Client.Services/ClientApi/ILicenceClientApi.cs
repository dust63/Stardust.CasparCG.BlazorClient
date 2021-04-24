using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{
   public interface ILicenceClientApi
    {

        [Get("/syncfusionlicence")]
        Task<ApiResponse<string>> GetSyncFusionLicence();


    }
}
