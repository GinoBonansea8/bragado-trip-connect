using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BragadoTripConnect.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleFareAndDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Schedules",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Schedules");
        }
    }
}
