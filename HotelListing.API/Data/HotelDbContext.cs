using HotelListing.API.Configurations;
using HotelListing.API.Configurations.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CountrySeedConfig());
        modelBuilder.ApplyConfiguration(new HotelSeedConfig());
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
}
