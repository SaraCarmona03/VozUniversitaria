using Microsoft.EntityFrameworkCore;
using VozUniversitaria.Models;

namespace VozUniversitaria.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(user => user.Email).IsUnique();
                entity.HasIndex(user => user.CaseNumber).IsUnique();

                entity.Property(user => user.Role)
                    .HasConversion<int>();

                entity.HasData(
                    new Users
                    {
                        Id = 1,
                        Name = "System",
                        LastName = "Administrator",
                        CaseNumber = "ADMIN-001",
                        Email = "admin@vozuniversitaria.local",
                        PasswordHash = "$2a$11$Kk5s/ejkGhy580CL6qhP2.BeGa7p.YDQkpxSHJV9/BzI4Ys4JzMj.",
                        Role = UserRole.Admin,
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Users
                    {
                        Id = 2,
                        Name = "Test",
                        LastName = "Teacher",
                        CaseNumber = "TEACHER-001",
                        Email = "teacher@vozuniversitaria.local",
                        PasswordHash = "$2a$11$Kk5s/ejkGhy580CL6qhP2.BeGa7p.YDQkpxSHJV9/BzI4Ys4JzMj.",
                        Role = UserRole.Teacher,
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    });
            });
        }
    }
}