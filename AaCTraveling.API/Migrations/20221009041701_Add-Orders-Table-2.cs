using Microsoft.EntityFrameworkCore.Migrations;

namespace AaCTraveling.API.Migrations
{
    public partial class AddOrdersTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "91f3e64f-e20d-417e-9466-71534a40410a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f78cb220-f051-4141-b0f5-4a6490225251", "AQAAAAEAACcQAAAAEBi/zVSFBBe2ftgnQEP9yZVdUZiDFdrabUBXhqGTR8WYyWOq5PZgAg0BpLKH+fv6mg==", "cbc611f1-3e21-45d8-a709-f43ceb0c570d" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "81d9df67-4f52-4456-95e3-5529166955a6");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cebdbef7-8ba9-4998-8c32-0779fc78b098", "AQAAAAEAACcQAAAAEKNodHA3hTYnZWFuE5I1mc/zRLpfW6GYPg5S1mb+mr9Rtz9eCAetwPGzmBBTWIYdHw==", "c9dae69a-8a3e-4ded-ade8-bb3017bf2bbc" });
        }
    }
}
