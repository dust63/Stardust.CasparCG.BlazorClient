using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Stardust.Flux.PublishApi.Models;
using Stardust.Flux.PublishApi.Youtube;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Stardust.Flux.Core;
using Stardust.Flux.Core.Configuration;

namespace Stardust.Flux.PublishApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                     .AddJsonOptions(x =>
                                 {
                                     x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                                 });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PublishApi", Version = "v1" });
            });

            services.AddSignalR();
            services.Configure<YoutubeApiOptions>(options => Configuration.GetSection(YoutubeApiOptions.SectionName).Bind(options));
            services.AddScoped<YoutubeAppService>();
            services.AddScoped<AuthenticateService>();
            services.AddScoped<YoutubeUploader>();
            services.AddTransient<YoutubeSignalRClient>();

            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString("Stardust.Hangfire")));

            services
            .AddEntityFrameworkNpgsql()
            .AddDbContext<Models.PublishContext>(
              options =>
              {
                  options.UseNpgsql(Configuration.GetConnectionString("Stardust.Publish"));
              });
            services.AddMassTransit(x =>
        {
            var configOption = Configuration.GetSection(typeof(RabbitMqHostConfiguration).Name).Get<RabbitMqHostConfiguration>();
            x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c =>
            {
                c.Host(configOption.Hostname, cfg =>
                {
                    cfg.Username(configOption.User);
                    cfg.Password(configOption.Password);
                });
                c.MessageTopology.SetEntityNameFormatter(new RabbitExchangeNameFormater());
                c.ConfigureEndpoints(context);
            }));
        });
            services.AddMassTransitHostedService();


            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(1));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublishApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));
            var options = new BackgroundJobServerOptions
            {
                ServerName = String.Format("{0}.{1}", Environment.MachineName, Guid.NewGuid().ToString())
            };
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<YoutubeUploadHub>("/chathub");
                endpoints.MapHangfireDashboard();

            });
        }
    }
}
