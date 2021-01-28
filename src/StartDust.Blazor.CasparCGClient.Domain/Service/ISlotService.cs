using StartDust.Blazor.CasparCGClient.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartDust.Blazor.CasparCGClient.Domain.Service
{
    public interface ISlotService
    {
        CasparCGSlot Add(CasparCGSlot slot);
        bool IsReachable(CasparCGSlot slot);
    } 
}
