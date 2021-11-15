using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AaCTraveling.API.Migrations
{
    public partial class addColumsIntoTouristRoutesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("0caaa19d-a930-4b95-bdba-7bbd07758b86"));

            migrationBuilder.AddColumn<int>(
                name: "DepartureCity",
                table: "TouristRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "TouristRoutes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelDays",
                table: "TouristRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TripType",
                table: "TouristRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureCity", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Rating", "Title", "TravelDays", "TripType", "UpdateTime" },
                values: new object[] { new Guid("29f8d5a9-14f7-4513-9239-e6816ea7602c"), new DateTime(2021, 11, 15, 9, 43, 17, 248, DateTimeKind.Utc).AddTicks(5315), null, null, "TestRouteDescription1", null, null, null, null, 10086m, null, "TestTesttestRoute1", null, null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("29f8d5a9-14f7-4513-9239-e6816ea7602c"));

            migrationBuilder.DropColumn(
                name: "DepartureCity",
                table: "TouristRoutes");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "TouristRoutes");

            migrationBuilder.DropColumn(
                name: "TravelDays",
                table: "TouristRoutes");

            migrationBuilder.DropColumn(
                name: "TripType",
                table: "TouristRoutes");

            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("0caaa19d-a930-4b95-bdba-7bbd07758b86"), new DateTime(2021, 11, 15, 8, 58, 26, 255, DateTimeKind.Utc).AddTicks(2107), null, "TestRouteDescription1", null, null, null, null, 10086m, "TestTesttestRoute1", null });
        }
    }
}
