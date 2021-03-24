using Microsoft.EntityFrameworkCore;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Models
{
    public class DataContext : DbContext
    {

        public DbSet<RecordSlot> RecordSlots { get; set; }

        public DbSet<LiveStreamSlot> LiveStreamSlot { get; set; }

        public DbSet<Server> Servers { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

            modelBuilder.Entity<OutputSlot>()
              .HasDiscriminator<string>("SlotType")
                  .HasValue<RecordSlot>("Record")
                  .HasValue<LiveStreamSlot>("LiveStream");
        }
    }
}