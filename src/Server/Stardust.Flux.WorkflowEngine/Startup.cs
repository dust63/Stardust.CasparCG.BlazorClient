using Elsa.Persistence.EntityFramework.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Elsa;
using Hangfire.PostgreSql;
using Elsa.Persistence.EntityFramework.Sqlite;
using Microsoft.EntityFrameworkCore;
using Stardust.Flux.DataAccess;

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
                   .AddActivitiesFrom<Startup>()
                   .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                   .AddEntityActivities()
                   .AddObsActivities()
                   .AddFTPActivities()               
                   .AddHangfireTemporalActivities(hangfire => hangfire.UsePostgreSqlStorage(Configuration.GetConnectionString("DefaultConnection")), (s, o) => { o.SchedulePollingInterval = TimeSpan.FromSeconds(1); })
               //.AddWorkflowsFrom<Startup>()
               //.AddWorkflow((srv)=> new StreamingEventWorkflow(SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromSeconds(5)), Duration.FromSeconds(10)))
               );
            services
                    .AddElsaSwagger()
                    .AddElsaApiEndpoints();

            services.AddEntityFrameworkNpgsql().AddDbContext<DataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(cors => cors.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("Content-Disposition")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors();
            app.UseStaticFiles();
            app.UseHttpActivities();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                // Elsa Server uses ASP.NET Core Controllers.
                endpoints.MapControllers();

                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }

   
}
