using Stardust.Flux.Contract;
using System.Threading;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract.Api
{
    public interface IServerApi
    {
        Task<PagedListDto<ServerDto>> Get(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100);

        Task<ServerDto> Get(CancellationToken cancellationToken, int serverId);
        Task<ServerDto> Insert(CancellationToken cancellationToken, ServerDto server);
        Task<ServerDto> Update(CancellationToken cancellationToken, ServerDto server);
    }
}