using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Stardust.Flux.DataAccess.Models;

namespace Stardust.Flux.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> contextOptions) : base(contextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .Property(e => e.ExtraParams)
                .HasJsonValueConversion();
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<YoutubeAccount> YoutubeAccounts { get; set; }
        public DbSet<YoutubeUpload> YoutubeUploads { get; internal set; }
    }
}