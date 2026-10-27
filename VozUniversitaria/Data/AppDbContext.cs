
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VozUniversitaria.Models;
namespace VozUniversitaria.Data
{
    public class AppDbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Nuevo modelo para el perfil académico del alumno
        public DbSet<StudentProfile> StudentProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Users>().ToTable("Users");

            // Cada cuenta tendrá como máximo un perfil
            builder.Entity<StudentProfile>()
                .HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<StudentProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
