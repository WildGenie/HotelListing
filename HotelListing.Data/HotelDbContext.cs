using HotelListing.API.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

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

/* ADDING DBCONTEXT FACTORY SCAFFOLDING ISSUES ARE RESOLVED
---------------------------------------------------------*/
public class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
{
    public HotelDbContext CreateDbContext(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
        var connect = config.GetConnectionString("HotelAPIDbString");
        
        optionsBuilder.UseSqlServer(connect);

        return new HotelDbContext(optionsBuilder.Options);
    }
}
