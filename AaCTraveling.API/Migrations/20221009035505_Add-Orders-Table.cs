using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AaCTraveling.API.Migrations
{
    public partial class AddOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TourisRouteId",
                table: "LineItems",
                newName: "TouristRouteId");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "LineItems",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreateDateUTC = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TransactionMetadata = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_TouristRouteId",
                table: "LineItems",
                column: "TouristRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_TouristRoutes_TouristRouteId",
                table: "LineItems",
                column: "TouristRouteId",
                principalTable: "TouristRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_TouristRoutes_TouristRouteId",
                table: "LineItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_TouristRouteId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "LineItems");

            migrationBuilder.RenameColumn(
                name: "TouristRouteId",
                table: "LineItems",
                newName: "TourisRouteId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2",
                column: "ConcurrencyStamp",
                value: "0cd9e308-6ad5-4a5d-9845-9b37af792931");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6e00b20-08a1-434d-9357-4095fbefc9bb", "AQAAAAEAACcQAAAAEF0CiYqAMIKJqj1fDDrcaMVTn5qX108QVSytXCcMe5A/INPXnnsrS0XOl74WRG8TDw==", "83c4b54f-aef4-4fb9-ae7f-70629d7c6b5d" });
        }
    }
}
