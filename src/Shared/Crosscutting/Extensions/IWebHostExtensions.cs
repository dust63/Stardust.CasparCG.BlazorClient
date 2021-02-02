using System.Threading;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Stardust.Flux.Crosscutting
{
    public static class IWebHostExtensions
    {

        public static IHost MigrateDatabase<T>(this IHost webHost, TimeSpan? maxWaitingForSql = null) where T : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {

                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<IHost>>();
                try
                {
                    var initialStartTime = DateTime.Now;

                    maxWaitingForSql = maxWaitingForSql ?? TimeSpan.FromMinutes(1);
                    var db = services.GetRequiredService<T>();
                    logger.LogInformation($"Initialize db context with {db.Database.GetConnectionString()}");
                    while (!db.Database.CanConnect())
                    {
                        logger.LogInformation("Waiting for the bdd to be ready");
                        Thread.Sleep(5000);
                        if (DateTime.Now - initialStartTime > maxWaitingForSql)
                            throw new InvalidOperationException("Can't contact the db. Timeout exceed");
                    }
                    logger.LogInformation("bdd is ready");
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return webHost;
        }

    }
}