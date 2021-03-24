using System.Threading;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract.CoreApi
{
    public interface IRecordSlotApi
    {
        Task<PagedListDto<RecordSlotDto>> GetList(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100);
        Task<RecordSlotDto> Get(CancellationToken cancellationToken, int slotId);
        Task<RecordSlotDto> Insert(CancellationToken cancellationToken, RecordSlotDto slot);
        Task<RecordSlotDto> Update(CancellationToken cancellationToken, RecordSlotDto slot);
    }
}