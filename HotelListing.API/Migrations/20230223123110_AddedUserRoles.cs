using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelListing.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1d3fe105-e5e9-8a1z-4e05-62a2105df6e4", null, "Administrator", "ADMINISTRATOR" },
                    { "e5e9e105-8a1z-4e05-62a2-105df6e41d3f", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "ConcurrencyStamp", "Country", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2", 0, 0, "ae3655f0-35a6-4b52-a2aa-ecc689e3bd4c", null, "admin@localhost.com", true, "System", "Admin", false, null, "ADMIN@LOCALHOST.COM", "ADMIN@LOCALHOST.COM", "AQAAAAIAAYagAAAAEPz4UKto+/VWOpoyuujDFUpZ3Xy7tYl7VnTqORtO5GC41XMgs6vOxl/s6ItN7lq1OQ==", null, false, "0df6d234-87dc-42b9-b542-ddc50b9a6ca6", false, "admin@localhost.com" },
                    { "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2", 0, 0, "45a2170d-88ef-4ae5-b514-c03b7c314416", null, "user@localhost.com", true, "User", "Test", false, null, "USER@LOCALHOST.COM", "USER@LOCALHOST.COM", "AQAAAAIAAYagAAAAEPShDQFQLxEOri/xnmx0tog7ROoZAdCeOpME8eDTK4OVwxiJtxRF/Tll2Dj7v1DL0w==", null, false, "d3b2a6f7-d294-4382-9c30-46567811f962", false, "user@localhost.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1d3fe105-e5e9-8a1z-4e05-62a2105df6e4", "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2" },
                    { "e5e9e105-8a1z-4e05-62a2-105df6e41d3f", "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1d3fe105-e5e9-8a1z-4e05-62a2105df6e4", "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e5e9e105-8a1z-4e05-62a2-105df6e41d3f", "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d3fe105-e5e9-8a1z-4e05-62a2105df6e4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5e9e105-8a1z-4e05-62a2-105df6e41d3f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "62b3adbb-e3a7-4f5f-8a05-61d2074df6c2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6c2adbb-4f5f-8a05-e3a7-62b3adbb61d2");
        }
    }
}
