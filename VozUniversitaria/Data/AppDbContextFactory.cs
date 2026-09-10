using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VozUniversitaria.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=VozUniversitariaDb;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}