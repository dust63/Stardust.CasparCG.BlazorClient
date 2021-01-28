using Microsoft.EntityFrameworkCore;
using StartDust.Blazor.CasparCGClient.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartDust.Blazor.CasparCGClient.Data
{


    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {            
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

        }
        public DbSet<CasparCgServer> CasparCgServers { get; set; }
    }
}
