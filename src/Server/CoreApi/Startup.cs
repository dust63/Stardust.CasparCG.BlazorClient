using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
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
using Stardust.Flux.CoreApi.Models;
using AutoMapper;

namespace Stardust.Flux.CoreApi
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreApi", Version = "v1" });
            });
            services
            .AddEntityFrameworkNpgsql()
            .AddDbContext<DataContext>(
              options =>
              {
                  options.UseNpgsql(Configuration.GetConnectionString("Stardust"));
              })
              .AddUnitOfWork<DataContext>()
              .AddAutoMapper(typeof(Startup));
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "Open",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader());
            });
            services.Configure<LicenceConfig>(Configuration.GetSection(nameof(LicenceConfig)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors("Open");
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
