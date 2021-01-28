using Microsoft.EntityFrameworkCore;

namespace ScheduleEngine.Models
{
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options) {

                
             }
    }
}