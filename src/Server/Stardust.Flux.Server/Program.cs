using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Stardust.Flux.Crosscutting;
using Stardust.Flux.DataAccess;
using Stardust.Flux.Server;

namespace Stardust.Flux.ScheduleEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
            .Build()
            .MigrateDatabase<DataContext>()         
            .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
