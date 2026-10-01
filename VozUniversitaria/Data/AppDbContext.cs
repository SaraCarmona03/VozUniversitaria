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
    }
}