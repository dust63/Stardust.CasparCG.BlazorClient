using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Stardust.Flux.Client.Services;
using Refit;
using Polly;

namespace Stardust.Flux.FrontEnd
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


            ManageRefitClient(builder);
            AddScheduleServices(builder);


            await builder.Build().RunAsync();
        }



        private static void ManageRefitClient(WebAssemblyHostBuilder builder)
        {
            builder.Services
                    .AddRefitClient<ILicenceClientApi>()
                    .ConfigureHttpClient(c =>
                    {
                        c.BaseAddress = new Uri(builder.Configuration["CoreApiUrl"]);
                    });
        }


        private static void AddScheduleServices(WebAssemblyHostBuilder builder)
        {
            if (builder.HostEnvironment.Environment == "Development")
            {

                builder.Services
                       .AddRefitClient<IRecordClientApi>()
                       .ConfigureHttpClient(c =>
                       {
                           c.BaseAddress = new Uri(builder.Configuration["ScheduleApiUrl"]);
                       });
            }
            else
            {
                builder.Services
                       .AddRefitClient<IRecordClientApi>()
                       .ConfigureHttpClient(c =>
                       {
                           c.BaseAddress = new Uri(builder.Configuration["ScheduleApiUrl"]);
                       })
                       //Retry policy using Polly
                       //You could also add a fallback policy, a circuit-breaker or any combination of these
                       .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                       .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                       }));

            }
            builder.Services.AddScoped<IRecordModelService, RecordModelService>();
        }

    }
}
