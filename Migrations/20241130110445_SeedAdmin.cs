using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a098f11-7609-4c74-9a43-0240c18b4895");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8f25dbb-d579-43b0-97dd-dc300d936581");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "970d493b-3d18-49e4-ba77-89644aec5672", null, "Admin", "ADMIN" },
                    { "b20af919-db9b-4cae-8b90-9c9e03b55efa", null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "3b2c9714-d4fa-4950-b4ea-4c97046ff854", 0, "4d262718-4881-4a10-b680-4d40b67d79da", "Admin", "yuossefbakier@gmail.com", true, false, null, "YUOSSEFBAKIER@GMAIL.COM", "YOUSEF", "AQAAAAIAAYagAAAAEPjG1uo79P3J5L6nDckSBIK+R83d08P/XXaGBwKYvQ7jXfnc3aU7OLOr4TibAGRx/Q==", null, false, "a7f4f7b4-931d-4d62-b9ca-1e3d88cf1da0", false, "Yousef" },
                    { "faebc153-217e-4469-813c-57feeab50607", 0, "da7d1c4b-cf2e-4a9b-acd5-bd87f1a0a6f5", "Admin", "admin@joo.com", true, false, null, "ADMIN@JOO.COM", "ADMIN", "AQAAAAIAAYagAAAAEK+9uBTxeExZ3p7sjI6BJJsL+vtnwGjsoZQ9rtdQOryzejnuLVbjJkttDSBA9aZ+XA==", null, false, "9862f628-b2d1-4acc-a222-69a5ed8d655b", false, "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "970d493b-3d18-49e4-ba77-89644aec5672", "3b2c9714-d4fa-4950-b4ea-4c97046ff854" },
                    { "970d493b-3d18-49e4-ba77-89644aec5672", "faebc153-217e-4469-813c-57feeab50607" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b20af919-db9b-4cae-8b90-9c9e03b55efa");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "970d493b-3d18-49e4-ba77-89644aec5672", "3b2c9714-d4fa-4950-b4ea-4c97046ff854" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "970d493b-3d18-49e4-ba77-89644aec5672", "faebc153-217e-4469-813c-57feeab50607" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "970d493b-3d18-49e4-ba77-89644aec5672");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3b2c9714-d4fa-4950-b4ea-4c97046ff854");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "faebc153-217e-4469-813c-57feeab50607");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5a098f11-7609-4c74-9a43-0240c18b4895", null, "Admin", "ADMIN" },
                    { "e8f25dbb-d579-43b0-97dd-dc300d936581", null, "Customer", "CUSTOMER" }
                });
        }
    }
}
