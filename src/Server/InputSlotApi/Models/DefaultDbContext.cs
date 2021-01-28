using Microsoft.EntityFrameworkCore;

namespace Stardust.Flux.InputSlotApi.Models
{
    public class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options)
        {


        }
    }
}