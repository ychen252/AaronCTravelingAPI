using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AaCTraveling.API.Migrations
{
    public partial class addSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("0caaa19d-a930-4b95-bdba-7bbd07758b86"), new DateTime(2021, 11, 15, 8, 58, 26, 255, DateTimeKind.Utc).AddTicks(2107), null, "TestRouteDescription1", null, null, null, null, 10086m, "TestTesttestRoute1", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("0caaa19d-a930-4b95-bdba-7bbd07758b86"));
        }
    }
}
