using Elsa.Persistence.EntityFramework.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa;
using Hangfire.PostgreSql;
using Elsa.Persistence.EntityFramework.Sqlite;
using Stardust.Flux.WorkflowEngine.Activities;
using Stardust.Flux.WorkflowEngine.Workflow;
using NodaTime;

namespace Stardust.Flux.WorkflowEngine
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
  
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            var elsaSection = Configuration.GetSection("Elsa");          
            services
               .AddElsa(elsa => elsa
                   .UseEntityFrameworkPersistence(ef => ef.UseSqlite(Configuration.GetConnectionString("Elsa")), true)
                   .AddHttpActivities(htttp=> htttp.BasePath = "/api")
                   .AddEntityActivities()
                   .AddObsActivities()
                   .AddHangfireTemporalActivities(hangfire => hangfire.UsePostgreSqlStorage(Configuration.GetConnectionString("DefaultConnection")),(s,o)=> { o.SchedulePollingInterval = TimeSpan.FromSeconds(1); })                  
                   //.AddWorkflowsFrom<Startup>()
                   //.AddWorkflow((srv)=> new StreamingEventWorkflow(SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromSeconds(5)), Duration.FromSeconds(10)))
               );
      
            services.AddElsaApiEndpoints();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app                  
                 .UseStaticFiles() // For Dashboard.
                 .UseHttpActivities()
                 .UseRouting()
                 .UseEndpoints(endpoints =>
                 {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    // For Dashboard.
                    endpoints.MapFallbackToPage("/_Host");
                 });
        }
    }
}
