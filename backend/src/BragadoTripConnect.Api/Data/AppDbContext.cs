using Microsoft.EntityFrameworkCore;

namespace BragadoTripConnect.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HealthCheck> HealthChecks => Set<HealthCheck>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Route> Routes => Set<Route>();

    public DbSet<Schedule> Schedules => Set<Schedule>();

    public DbSet<Booking> Bookings => Set<Booking>();

    // Every table is keyed by an autoincrement Id, which EF Core picks up from
    // the property name without being told. The business identifiers the keys
    // used to be — CUIT, DNI, origin and destination — are still unique, but
    // as unique indexes, so that changing one is an update rather than a new
    // row every other table has to be pointed at again.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HealthCheck>().HasData(new HealthCheck
        {
            Id = 1,
            Status = "ok",
            CheckedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });

        modelBuilder.Entity<Company>().HasIndex(company => company.Cuit).IsUnique();

        // The operators and the routes they serve barely ever change, so they
        // ship with the database instead of needing screens to manage them.
        // The CUITs below are placeholders — the real ones still have to be
        // looked up and filled in before this runs against anything but a
        // development database.
        modelBuilder.Entity<Company>().HasData(
            new Company { Id = 1, Cuit = "30-00000001-7", Name = "21900" },
            new Company { Id = 2, Cuit = "30-00000002-5", Name = "Santorini Turismo" },
            new Company { Id = 3, Cuit = "30-00000003-3", Name = "Chevallier" },
            new Company { Id = 4, Cuit = "30-00000004-1", Name = "Trenes Argentinos" });

        modelBuilder.Entity<Route>()
            .HasIndex(route => new { route.Origin, route.Destination })
            .IsUnique();

        modelBuilder.Entity<Route>().HasData(
            new Route { Id = 1, Origin = "Bragado", Destination = "Retiro" },
            new Route { Id = 2, Origin = "Retiro", Destination = "Bragado" },
            new Route { Id = 3, Origin = "Bragado", Destination = "Once" },
            new Route { Id = 4, Origin = "Once", Destination = "Bragado" });

        modelBuilder.Entity<User>().HasIndex(user => user.Dni).IsUnique();

        modelBuilder.Entity<Schedule>(schedule =>
        {
            schedule.Property(entity => entity.Price).HasPrecision(10, 2);

            // What the old five-column key ruled out, kept as a rule of its
            // own: one company cannot publish two departures on the same
            // route, date and time.
            schedule.HasIndex(entity => new
            {
                entity.CompanyId,
                entity.RouteId,
                entity.Date,
                entity.Time
            }).IsUnique();

            schedule.HasOne(entity => entity.Route)
                .WithMany()
                .HasForeignKey(entity => entity.RouteId);

            schedule.HasOne(entity => entity.Company)
                .WithMany()
                .HasForeignKey(entity => entity.CompanyId);
        });

        modelBuilder.Entity<Booking>(booking =>
        {
            // Deliberately no unique index on (ScheduleId, UserId). The old
            // six-column key made "one booking per person per departure" a
            // rule by accident, and whether it should be one — a passenger
            // booking a second seat, or booking for a relative — has not been
            // decided yet.
            booking.HasOne(entity => entity.Schedule)
                .WithMany()
                .HasForeignKey(entity => entity.ScheduleId);

            booking.HasOne(entity => entity.User)
                .WithMany()
                .HasForeignKey(entity => entity.UserId);
        });
    }
}
