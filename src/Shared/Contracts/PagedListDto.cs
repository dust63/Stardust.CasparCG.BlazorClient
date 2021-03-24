using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Contract
{
    public record PagedListDto<T>(int IndexFrom, int PageIndex, int PageSize, int TotalCount, int TotalPages, IList<T> items,bool HasPreviousPage,bool HasNextPage);   
}
