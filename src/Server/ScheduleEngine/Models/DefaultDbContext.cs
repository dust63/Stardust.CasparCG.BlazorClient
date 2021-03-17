using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}