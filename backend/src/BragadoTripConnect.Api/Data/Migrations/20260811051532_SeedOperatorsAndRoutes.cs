using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BragadoTripConnect.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedOperatorsAndRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Companies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Cuit", "Name" },
                values: new object[,]
                {
                    { "30-00000001-7", "21900" },
                    { "30-00000002-5", "Santorini Turismo" },
                    { "30-00000003-3", "Chevallier" },
                    { "30-00000004-1", "Trenes Argentinos" }
                });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "Destination", "Origin" },
                values: new object[,]
                {
                    { "Once", "Bragado" },
                    { "Retiro", "Bragado" },
                    { "Bragado", "Once" },
                    { "Bragado", "Retiro" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Cuit",
                keyValue: "30-00000001-7");

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Cuit",
                keyValue: "30-00000002-5");

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Cuit",
                keyValue: "30-00000003-3");

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Cuit",
                keyValue: "30-00000004-1");

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumns: new[] { "Destination", "Origin" },
                keyValues: new object[] { "Once", "Bragado" });

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumns: new[] { "Destination", "Origin" },
                keyValues: new object[] { "Retiro", "Bragado" });

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumns: new[] { "Destination", "Origin" },
                keyValues: new object[] { "Bragado", "Once" });

            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumns: new[] { "Destination", "Origin" },
                keyValues: new object[] { "Bragado", "Retiro" });

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Companies");
        }
    }
}
