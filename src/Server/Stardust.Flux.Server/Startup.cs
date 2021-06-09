using System;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Stardust.Flux.Contract.DTO.Schedule;
using Stardust.Flux.DataAccess;
using Stardust.Flux.ScheduleEngine.Services;
using Stardust.Flux.Server.Extensions;

namespace Stardust.Flux.Server
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
         
            services.AddEntityFrameworkNpgsql().AddDbContext<DataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
                   
            services.AddEventServices();
          
            services.AddYoutube(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader());
            });
        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IHostApplicationLifetime lifetime)
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
            app.UseHangfireDashboard()
                .UseHangfireServer(new BackgroundJobServerOptions {ServerName = GetType().Namespace });
          
            app.UseCors("Open");
            serviceProvider.GetService<IEventSchedulerService<RecordParameters>>().StopAllMissedStop();
            app.UseEndpoints(endpoints =>
            {              
                endpoints.MapControllers();              
            });
        }
    }
}
