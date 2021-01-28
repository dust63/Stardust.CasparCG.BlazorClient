

using StartDust.Blazor.CasparCGClient.Core;

namespace StartDust.Blazor.CasparCGClient.Domain.Service
{
    public interface IScheduleRecordService
    {
        ISchedule Add(ISchedule scheduleRecord);
    }
}
