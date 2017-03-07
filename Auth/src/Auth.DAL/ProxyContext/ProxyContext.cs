using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.DAL.ProxyContext.Enteities;
using Microsoft.EntityFrameworkCore;

namespace Auth.DAL.ProxyContext
{
    public class ProxyContext : DbContext
    {
        public ProxyContext(DbContextOptions<ProxyContext> options) : base(options)
        {
        }

        public DbSet<ProxyDomain> Domains { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
