using Microsoft.EntityFrameworkCore;

namespace Stardust.Flux.ScheduleEngine.Models
{
    public class ScheduleContext : DbContext
    {


        public DbSet<RecordJob> RecordJobs { get; set; }
        public ScheduleContext(DbContextOptions<ScheduleContext> options)
            : base(options)
        {


        }
    }
}