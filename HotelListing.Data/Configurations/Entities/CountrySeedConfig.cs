using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Configurations.Entities;

public class CountrySeedConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasData(
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
    }
}
