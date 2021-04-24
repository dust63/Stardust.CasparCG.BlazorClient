using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;

namespace Stardust.Flux.ScheduleEngine.Models
{
    public class ScheduleContext : DbContext
    {


        public DbSet<Event> Events { get; set; }
        public ScheduleContext(DbContextOptions<ScheduleContext> options)
            : base(options)
        {


        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .Property(e => e.ExtraParams)
                .HasJsonValueConversion( );
        }
    }

   
}