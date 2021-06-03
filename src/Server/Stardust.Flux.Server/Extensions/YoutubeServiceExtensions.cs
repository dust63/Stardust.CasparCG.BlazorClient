using Microsoft.Extensions.DependencyInjection;
using Stardust.Flux.Server.Options;
using Stardust.Flux.Server.Services;
using Stardust.Flux.Server.Services.Youtube;
using Microsoft.Extensions.Configuration;


namespace Stardust.Flux.Server.Extensions
{
    public static class YoutubeServiceExtensions
    {

        public static IServiceCollection AddYoutube(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSignalR();
            services.Configure<YoutubeApiOptions>(options => configuration.GetSection(YoutubeApiOptions.SectionName).Bind(options));
            services.AddScoped<YoutubeAppService>();
            services.AddMemoryCache();
            services.AddScoped<ITempStorage, TempStorage>();
            services.AddScoped<AuthentificationService>();
            services.AddScoped<YoutubeUploader>();
            services.AddTransient<YoutubeSignalRClient>();

            return services;
        }
    }
}
