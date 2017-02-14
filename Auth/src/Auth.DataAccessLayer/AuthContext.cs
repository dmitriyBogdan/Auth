using Auth.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.DataAccessLayer
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }

        public DbSet<LocalUser> LocalUsers { get; set; }

        public DbSet<ExternalUser> ExternalUsers { get; set; }

        public DbSet<Claim> Claims { get; set; }

        public DbSet<LocalUserClaim> LocalUserClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocalUserClaim>().HasKey(x => new { x.LocalUserId, x.ClaimId });
        }
    }
}