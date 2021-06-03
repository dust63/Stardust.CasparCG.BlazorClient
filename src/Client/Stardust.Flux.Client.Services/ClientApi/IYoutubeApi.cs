using Refit;
using Stardust.Flux.Contract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{
    [Headers("Content-Type: application/json")]
    public interface IYoutubeApi
    {
        [Get("/Youtube/Account")]
        Task<IApiResponse<IDictionary<string, string>>> GetAccountsInfo(int pageIndex, int pageSize);



        [Get("/Youtube/ChannelInfo")]
        Task<IApiResponse<YoutubeChannelDto[]>> GetChannelInfo(string accountId);

        [Post("​/Youtube​/Account​/Grant")]
        Task<IApiResponse> AddChannel(string name, string accountId = null);

    }
}
