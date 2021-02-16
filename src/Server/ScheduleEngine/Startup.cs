using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Stardust.Flux.ScheduleEngine.Models;
using Stardust.Flux.ScheduleEngine.Services;

namespace Stardust.Flux.ScheduleEngine
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScheduleEngine", Version = "v1" });
            });
            services.AddEntityFrameworkNpgsql().AddDbContext<ScheduleContext>(
                options =>
                {
                    options.UseNpgsql(Configuration.GetConnectionString("Stardust"));
                });

            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString("Stardust")));
            services.AddTransient<IRecordSchedulerService, RecordSchedulerService>();
            services.AddTransient<IRecordControler, DummyRecordController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleEngine v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));
            //Will be available under http://localhost:5000/hangfire"
            app.UseHangfireDashboard();
            var options = new BackgroundJobServerOptions
            {
                SchedulePollingInterval = TimeSpan.FromSeconds(1),
                WorkerCount = 50
            };
            app.UseHangfireServer(options);
            serviceProvider.GetService<IRecordSchedulerService>().StopAllMissedStop();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
