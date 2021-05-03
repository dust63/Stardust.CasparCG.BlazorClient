using Refit;
using Stardust.Flux.Contract;
using Stardust.Flux.Contract.ApiContracts;
using Stardust.Flux.Contract.DTO.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{

    [Headers("Content-Type: application/json")]
    public interface ICasparDataApi
    {


        [Get("/CasparCG/template/list")]
        Task<ApiResponse<IList<TemplateDto>>> GetTemplates();

        [Get("​/CasparCG​/movie​/list")]
        Task<ApiResponse<IList<MovieDto>>> GetMovies();


    }
}
