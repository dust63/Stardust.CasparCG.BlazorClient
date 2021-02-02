using Microsoft.EntityFrameworkCore;

namespace Stardust.Flux.ScheduleEngine.Models
{
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options) {

                
             }
    }
}