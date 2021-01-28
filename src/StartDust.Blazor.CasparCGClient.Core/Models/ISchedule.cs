using System;

namespace StartDust.Blazor.CasparCGClient.Core
{
    public interface ISchedule
    {
        RecuringType RecuringType { get; }

        DateTime StartTime { get; }

        TimeSpan Duration { get; }

        string CronExpression { get; }

    }
   
}
