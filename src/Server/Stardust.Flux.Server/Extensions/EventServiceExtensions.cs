using Microsoft.Extensions.DependencyInjection;
using Stardust.Flux.Contract.DTO.Schedule;
using Stardust.Flux.ScheduleEngine.Services;
using Stardust.Flux.Server.Services;

namespace Stardust.Flux.Server.Extensions
{
    public static class EventServiceExtensions
    {

        public static void AddEventServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IEventSchedulerService<>), typeof(EventSchedulerService<>));
            services.AddTransient(typeof(IEventConsumer<RecordParameters>), typeof(DummyRecordController));
            services.AddTransient(typeof(IEventNotificationService),typeof(EventNotificationService));
            services.AddScoped<IEventNotificationService, EventNotificationService>();
        }
    }
}
