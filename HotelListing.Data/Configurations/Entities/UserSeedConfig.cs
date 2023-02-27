using HotelListing.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Configurations.Entities;

/* SETTING ADMIN USER FOR SYSTEM 
------------------------------*/

public class UserSeedConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var hash = new PasswordHasher<User>();
        builder.HasData(
            new User
            {
                Id = "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2",
                Email = "admin@localhost.com",
                NormalizedEmail = "ADMIN@LOCALHOST.COM",
                UserName = "admin@localhost.com",
                NormalizedUserName = "ADMIN@LOCALHOST.COM",
                FirstName = "System",
                LastName = "Admin",
                Country = "Netherlands",
                PasswordHash = hash.HashPassword(null, "P@ssW0rd123!"),
                EmailConfirmed = true
            },
            new User
            {
                Id = "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2",
                Email = "user@localhost.com",
                NormalizedEmail = "USER@LOCALHOST.COM",
                UserName = "user@localhost.com",
                NormalizedUserName = "USER@LOCALHOST.COM",
                FirstName = "User",
                LastName = "Test",
                Country = "Netherlands",
                PasswordHash = hash.HashPassword(null, "P@ssW0rd123!"),
                EmailConfirmed = true
            }
        );
    }
}