using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BragadoTripConnect.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Cuit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Cuit);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Origin = table.Column<string>(type: "text", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => new { x.Origin, x.Destination });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Dni = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Dni);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    RouteOrigin = table.Column<string>(type: "text", nullable: false),
                    RouteDestination = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CompanyCuit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => new { x.RouteOrigin, x.RouteDestination, x.Date, x.Time, x.CompanyCuit });
                    table.ForeignKey(
                        name: "FK_Schedules_Companies_CompanyCuit",
                        column: x => x.CompanyCuit,
                        principalTable: "Companies",
                        principalColumn: "Cuit",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Routes_RouteOrigin_RouteDestination",
                        columns: x => new { x.RouteOrigin, x.RouteDestination },
                        principalTable: "Routes",
                        principalColumns: new[] { "Origin", "Destination" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    ScheduleRouteOrigin = table.Column<string>(type: "text", nullable: false),
                    ScheduleRouteDestination = table.Column<string>(type: "text", nullable: false),
                    ScheduleDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ScheduleTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ScheduleCompanyCuit = table.Column<string>(type: "text", nullable: false),
                    UserDni = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => new { x.ScheduleRouteOrigin, x.ScheduleRouteDestination, x.ScheduleDate, x.ScheduleTime, x.ScheduleCompanyCuit, x.UserDni });
                    table.ForeignKey(
                        name: "FK_Bookings_Schedules_ScheduleRouteOrigin_ScheduleRouteDestina~",
                        columns: x => new { x.ScheduleRouteOrigin, x.ScheduleRouteDestination, x.ScheduleDate, x.ScheduleTime, x.ScheduleCompanyCuit },
                        principalTable: "Schedules",
                        principalColumns: new[] { "RouteOrigin", "RouteDestination", "Date", "Time", "CompanyCuit" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserDni",
                        column: x => x.UserDni,
                        principalTable: "Users",
                        principalColumn: "Dni",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserDni",
                table: "Bookings",
                column: "UserDni");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CompanyCuit",
                table: "Schedules",
                column: "CompanyCuit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
