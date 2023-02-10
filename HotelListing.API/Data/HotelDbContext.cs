using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Netherlands",
                    CountryCode = "NL"
                },
                new Country
                {
                    Id = 2,
                    Name = "Belgium",
                    CountryCode = "BE"
                },
                new Country
                {
                    Id = 3,
                    Name = "Germany",
                    CountryCode = "GR"
                }
            );
        modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Hotel Hollandia",
                    Adress = "Amsterdam",
                    CountryId = 1,
                    Rating = 4.5
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Hotel Anvers",
                    Adress = "Bruxelles",
                    CountryId = 2,
                    Rating = 4.3
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Hotel Germania",
                    Adress = "Berlin",
                    CountryId = 3,
                    Rating = 4.4
                }
            );
    }
}
