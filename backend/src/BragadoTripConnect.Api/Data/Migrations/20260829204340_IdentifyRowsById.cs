using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BragadoTripConnect.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentifyRowsById : Migration
    {
        // Scaffolded, then rewritten by hand. Left alone, it dropped the seeded
        // operators and routes and reinserted them with ids, which left every
        // published schedule pointing at a company that no longer existed. The
        // ids are handed out below instead, and the rows that referenced a CUIT
        // or an origin/destination pair are pointed at them before the old
        // columns go away. Bookings is empty in every database this runs
        // against — nothing books a trip yet — so it only changes shape.
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Schedules_ScheduleRouteOrigin_ScheduleRouteDestina~",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserDni",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Companies_CompanyCuit",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Routes_RouteOrigin_RouteDestination",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(name: "PK_Bookings", table: "Bookings");
            migrationBuilder.DropPrimaryKey(name: "PK_Schedules", table: "Schedules");
            migrationBuilder.DropPrimaryKey(name: "PK_Users", table: "Users");
            migrationBuilder.DropPrimaryKey(name: "PK_Routes", table: "Routes");
            migrationBuilder.DropPrimaryKey(name: "PK_Companies", table: "Companies");

            migrationBuilder.DropIndex(name: "IX_Schedules_CompanyCuit", table: "Schedules");
            migrationBuilder.DropIndex(name: "IX_Bookings_UserDni", table: "Bookings");

            AddIdColumn(migrationBuilder, "Companies");
            AddIdColumn(migrationBuilder, "Routes");
            AddIdColumn(migrationBuilder, "Users");
            AddIdColumn(migrationBuilder, "Schedules");
            AddIdColumn(migrationBuilder, "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // The seeded rows are given the same ids the model seeds them with,
            // so that a database migrated from the old shape and one created
            // from scratch end up identical.
            migrationBuilder.Sql(@"
                UPDATE ""Companies"" SET ""Id"" = CASE ""Cuit""
                    WHEN '30-00000001-7' THEN 1
                    WHEN '30-00000002-5' THEN 2
                    WHEN '30-00000003-3' THEN 3
                    WHEN '30-00000004-1' THEN 4
                END;");

            migrationBuilder.Sql(@"
                UPDATE ""Routes"" SET ""Id"" = CASE
                    WHEN ""Origin"" = 'Bragado' AND ""Destination"" = 'Retiro' THEN 1
                    WHEN ""Origin"" = 'Retiro' AND ""Destination"" = 'Bragado' THEN 2
                    WHEN ""Origin"" = 'Bragado' AND ""Destination"" = 'Once' THEN 3
                    WHEN ""Origin"" = 'Once' AND ""Destination"" = 'Bragado' THEN 4
                END;");

            // Schedules have no natural order to preserve, so they are numbered
            // as they come. ctid is the physical location of the row, which is
            // the only handle on it while the table has no key at all.
            migrationBuilder.Sql(@"
                UPDATE ""Schedules"" AS s SET ""Id"" = numbered.""RowNumber""
                FROM (SELECT ctid, row_number() OVER () AS ""RowNumber"" FROM ""Schedules"") AS numbered
                WHERE s.ctid = numbered.ctid;");

            RestartIdentity(migrationBuilder, "Companies");
            RestartIdentity(migrationBuilder, "Routes");
            RestartIdentity(migrationBuilder, "Schedules");

            migrationBuilder.Sql(@"
                UPDATE ""Schedules"" AS s SET ""CompanyId"" = c.""Id""
                FROM ""Companies"" AS c WHERE c.""Cuit"" = s.""CompanyCuit"";");

            migrationBuilder.Sql(@"
                UPDATE ""Schedules"" AS s SET ""RouteId"" = r.""Id""
                FROM ""Routes"" AS r
                WHERE r.""Origin"" = s.""RouteOrigin"" AND r.""Destination"" = s.""RouteDestination"";");

            migrationBuilder.DropColumn(name: "RouteOrigin", table: "Schedules");
            migrationBuilder.DropColumn(name: "RouteDestination", table: "Schedules");
            migrationBuilder.DropColumn(name: "CompanyCuit", table: "Schedules");

            migrationBuilder.DropColumn(name: "ScheduleRouteOrigin", table: "Bookings");
            migrationBuilder.DropColumn(name: "ScheduleRouteDestination", table: "Bookings");
            migrationBuilder.DropColumn(name: "ScheduleDate", table: "Bookings");
            migrationBuilder.DropColumn(name: "ScheduleTime", table: "Bookings");
            migrationBuilder.DropColumn(name: "ScheduleCompanyCuit", table: "Bookings");
            migrationBuilder.DropColumn(name: "UserDni", table: "Bookings");

            migrationBuilder.AddPrimaryKey(name: "PK_Companies", table: "Companies", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Routes", table: "Routes", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Users", table: "Users", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Schedules", table: "Schedules", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Bookings", table: "Bookings", column: "Id");

            // What the natural keys used to enforce, now enforced on its own.
            migrationBuilder.CreateIndex(
                name: "IX_Companies_Cuit",
                table: "Companies",
                column: "Cuit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Origin_Destination",
                table: "Routes",
                columns: new[] { "Origin", "Destination" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Dni",
                table: "Users",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CompanyId_RouteId_Date_Time",
                table: "Schedules",
                columns: new[] { "CompanyId", "RouteId", "Date", "Time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_RouteId",
                table: "Schedules",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Companies_CompanyId",
                table: "Schedules",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Routes_RouteId",
                table: "Schedules",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Schedules_ScheduleId",
                table: "Bookings",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Schedules_Companies_CompanyId", table: "Schedules");
            migrationBuilder.DropForeignKey(name: "FK_Schedules_Routes_RouteId", table: "Schedules");
            migrationBuilder.DropForeignKey(name: "FK_Bookings_Schedules_ScheduleId", table: "Bookings");
            migrationBuilder.DropForeignKey(name: "FK_Bookings_Users_UserId", table: "Bookings");

            migrationBuilder.DropPrimaryKey(name: "PK_Bookings", table: "Bookings");
            migrationBuilder.DropPrimaryKey(name: "PK_Schedules", table: "Schedules");
            migrationBuilder.DropPrimaryKey(name: "PK_Users", table: "Users");
            migrationBuilder.DropPrimaryKey(name: "PK_Routes", table: "Routes");
            migrationBuilder.DropPrimaryKey(name: "PK_Companies", table: "Companies");

            migrationBuilder.DropIndex(name: "IX_Companies_Cuit", table: "Companies");
            migrationBuilder.DropIndex(name: "IX_Routes_Origin_Destination", table: "Routes");
            migrationBuilder.DropIndex(name: "IX_Users_Dni", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Schedules_CompanyId_RouteId_Date_Time", table: "Schedules");
            migrationBuilder.DropIndex(name: "IX_Schedules_RouteId", table: "Schedules");
            migrationBuilder.DropIndex(name: "IX_Bookings_ScheduleId", table: "Bookings");
            migrationBuilder.DropIndex(name: "IX_Bookings_UserId", table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "RouteOrigin",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RouteDestination",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyCuit",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleRouteOrigin",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleRouteDestination",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduleDate",
                table: "Bookings",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduleTime",
                table: "Bookings",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "ScheduleCompanyCuit",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserDni",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            // The same translation as Up, read the other way round.
            migrationBuilder.Sql(@"
                UPDATE ""Schedules"" AS s
                SET ""CompanyCuit"" = c.""Cuit""
                FROM ""Companies"" AS c WHERE c.""Id"" = s.""CompanyId"";");

            migrationBuilder.Sql(@"
                UPDATE ""Schedules"" AS s
                SET ""RouteOrigin"" = r.""Origin"", ""RouteDestination"" = r.""Destination""
                FROM ""Routes"" AS r WHERE r.""Id"" = s.""RouteId"";");

            migrationBuilder.DropColumn(name: "CompanyId", table: "Schedules");
            migrationBuilder.DropColumn(name: "RouteId", table: "Schedules");
            migrationBuilder.DropColumn(name: "ScheduleId", table: "Bookings");
            migrationBuilder.DropColumn(name: "UserId", table: "Bookings");

            migrationBuilder.DropColumn(name: "Id", table: "Companies");
            migrationBuilder.DropColumn(name: "Id", table: "Routes");
            migrationBuilder.DropColumn(name: "Id", table: "Users");
            migrationBuilder.DropColumn(name: "Id", table: "Schedules");
            migrationBuilder.DropColumn(name: "Id", table: "Bookings");

            migrationBuilder.AddPrimaryKey(name: "PK_Companies", table: "Companies", column: "Cuit");
            migrationBuilder.AddPrimaryKey(name: "PK_Users", table: "Users", column: "Dni");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routes",
                table: "Routes",
                columns: new[] { "Origin", "Destination" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                columns: new[] { "RouteOrigin", "RouteDestination", "Date", "Time", "CompanyCuit" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                columns: new[] { "ScheduleRouteOrigin", "ScheduleRouteDestination", "ScheduleDate", "ScheduleTime", "ScheduleCompanyCuit", "UserDni" });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CompanyCuit",
                table: "Schedules",
                column: "CompanyCuit");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserDni",
                table: "Bookings",
                column: "UserDni");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Companies_CompanyCuit",
                table: "Schedules",
                column: "CompanyCuit",
                principalTable: "Companies",
                principalColumn: "Cuit",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Routes_RouteOrigin_RouteDestination",
                table: "Schedules",
                columns: new[] { "RouteOrigin", "RouteDestination" },
                principalTable: "Routes",
                principalColumns: new[] { "Origin", "Destination" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Schedules_ScheduleRouteOrigin_ScheduleRouteDestina~",
                table: "Bookings",
                columns: new[] { "ScheduleRouteOrigin", "ScheduleRouteDestination", "ScheduleDate", "ScheduleTime", "ScheduleCompanyCuit" },
                principalTable: "Schedules",
                principalColumns: new[] { "RouteOrigin", "RouteDestination", "Date", "Time", "CompanyCuit" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserDni",
                table: "Bookings",
                column: "UserDni",
                principalTable: "Users",
                principalColumn: "Dni",
                onDelete: ReferentialAction.Cascade);
        }

        private static void AddIdColumn(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: table,
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        // The rows above were numbered by hand, which the sequence behind the
        // identity column knows nothing about. Without this the next insert
        // would ask for 1 and collide. An empty table is left starting at 1.
        private static void RestartIdentity(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.Sql($@"
                SELECT setval(
                    pg_get_serial_sequence('""{table}""', 'Id'),
                    COALESCE((SELECT MAX(""Id"") FROM ""{table}""), 1),
                    (SELECT COUNT(*) > 0 FROM ""{table}""));");
        }
    }
}
