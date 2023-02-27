using HotelListing.API.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data;

public class HotelDbContext : IdentityDbContext<User>
{
    public HotelDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CountrySeedConfig());
        modelBuilder.ApplyConfiguration(new HotelSeedConfig());

        modelBuilder.ApplyConfiguration(new RoleSeedConfig());
        modelBuilder.ApplyConfiguration(new UserRoleSeedConfig());
        modelBuilder.ApplyConfiguration(new UserSeedConfig());
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Country> Countries { get; set; }
}
