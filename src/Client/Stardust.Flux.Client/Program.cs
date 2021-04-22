using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;
using System.Net.Http;

using System.Threading.Tasks;
using Refit;
using Polly;
using Stardust.Flux.Client.Services;
using Stardust.Flux.ClientServices;

namespace Stardust.Flux.Client
{
    public partial class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddScoped<ThemeState>();
            builder.Services.AddScoped<IRecordModelService, RecordModelService>();

            if (builder.HostEnvironment.Environment == "Development")
            {

                builder.Services
                       .AddRefitClient<IRecordClientApi>()
                       .ConfigureHttpClient(c =>
                       {
                           c.BaseAddress = new Uri("https://localhost:44352");
                       });
            }
            else
            {
                builder.Services
                       .AddRefitClient<IRecordClientApi>()
                       .ConfigureHttpClient(c =>
                       {
                           c.BaseAddress = new Uri("http://localhost:34158");
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


          
            await builder.Build().RunAsync();

        }
    }
}
