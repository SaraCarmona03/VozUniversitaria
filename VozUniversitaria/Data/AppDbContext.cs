using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VozUniversitaria.Models;

namespace VozUniversitaria.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(user => user.Email).IsUnique();
                entity.HasIndex(user => user.CaseNumber).IsUnique();
                entity.Property(user => user.Role).HasConversion<int>();
            });
        }
    }
}