using CountriesAndCities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CountriesAndCities.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>()
                .HasOne(c => c.Country)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            var builder = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            IConfiguration config = builder.Build();
            var connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString:connectionString);
        }
    }
}
