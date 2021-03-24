using System.Threading;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract.CoreApi
{
    public interface ILiveSlotApi
    {
        Task<PagedListDto<LiveStreamSlotDto>> GetList(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100);
        Task<LiveStreamSlotDto> Get(CancellationToken cancellationToken, int slotId);
        Task<LiveStreamSlotDto> Insert(CancellationToken cancellationToken, RecordSlotDto slot);
        Task<LiveStreamSlotDto> Update(CancellationToken cancellationToken, RecordSlotDto slot);
    }
}