using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Configurations.Entities;

public class HotelSeedConfig : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData(
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
