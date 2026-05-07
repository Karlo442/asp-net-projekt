using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelperZaOptimalnuKupnju.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Svježa kokošja jaja", "Jaja XL", 2.50m },
                    { 2, "Svježi file lososa 200g", "Losos file", 15.00m },
                    { 3, "Pasirano mlijeko 1L", "Mlijeko 1L", 1.99m }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address", "Brand", "City", "Country", "Name", "OpeningHours" },
                values: new object[,]
                {
                    { 1, "Ilica 1", "Konzum", "Zagreb", "HR", "Konzum Kaptol", "07:00-21:00" },
                    { 2, "Trg 5", "Lidl", "Split", "HR", "Lidl Centar", "07:00-22:00" },
                    { 3, "Korzo 10", "Spar", "Rijeka", "HR", "Spar Rijeka", "08:00-20:00" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "IsActive", "LastName", "Phone", "RegisteredAt", "Role" },
                values: new object[,]
                {
                    { 1, "ivan.horvat@email.com", "Ivan", true, "Horvat", "+38591123456", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 2, "ana.kovac@email.com", "Ana", true, "Kovač", "+38591765432", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 3, "marko.babic@email.com", "Marko", true, "Babić", "+38591345678", new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, "petra.juric@email.com", "Petra", true, "Jurić", "+38591876543", new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "BuyerId", "CreatedAt", "ExpectedDeliveryDateTime", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 15.00m },
                    { 2, 2, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 69.90m },
                    { 3, 1, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 90.00m }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 10, 1.50m },
                    { 2, 2, 3, 10, 6.99m },
                    { 3, 3, 2, 2, 45.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
