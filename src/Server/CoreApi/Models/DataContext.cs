using Microsoft.EntityFrameworkCore;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Models
{
    public class DataContext : DbContext
    {

        public DbSet<Slot> Slots { get; set; }

        public DbSet<AdditionalSlotData> AdditionalSlotData { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}