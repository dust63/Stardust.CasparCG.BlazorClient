using Stardust.Flux.ScheduleEngine.Models;
using System;

namespace Stardust.Flux.ScheduleEngine.Services
{
    public interface IEventConsumer<T>
    {
        void Start(string eventId, TimeSpan duration,T parameters);
        void Stop(string eventId, T parameters);
    }
}