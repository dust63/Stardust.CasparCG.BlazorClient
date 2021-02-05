using Microsoft.EntityFrameworkCore;

namespace Stardust.Flux.PublishApi.Models
{
    public class PublishContext : DbContext
    {
        public PublishContext()
        {

        }

        public PublishContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<YoutubeAccount> YoutubeAccounts { get; set; }
        public DbSet<YoutubeUpload> YoutubeUploads { get; internal set; }
    }
}