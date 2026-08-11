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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HealthCheck>().HasData(new HealthCheck
        {
            Id = 1,
            Status = "ok",
            CheckedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });

        modelBuilder.Entity<Company>().HasKey(company => company.Cuit);

        // The operators and the routes they serve barely ever change, so they
        // ship with the database instead of needing screens to manage them.
        // The CUITs below are placeholders — the real ones still have to be
        // looked up and filled in before this runs against anything but a
        // development database.
        modelBuilder.Entity<Company>().HasData(
            new Company { Cuit = "30-00000001-7", Name = "21900" },
            new Company { Cuit = "30-00000002-5", Name = "Santorini Turismo" },
            new Company { Cuit = "30-00000003-3", Name = "Chevallier" },
            new Company { Cuit = "30-00000004-1", Name = "Trenes Argentinos" });

        modelBuilder.Entity<Route>().HasData(
            new Route { Origin = "Bragado", Destination = "Retiro" },
            new Route { Origin = "Retiro", Destination = "Bragado" },
            new Route { Origin = "Bragado", Destination = "Once" },
            new Route { Origin = "Once", Destination = "Bragado" });

        modelBuilder.Entity<User>().HasKey(user => user.Dni);

        modelBuilder.Entity<Route>().HasKey(route => new { route.Origin, route.Destination });

        modelBuilder.Entity<Schedule>(schedule =>
        {
            schedule.HasKey(entity => new
            {
                entity.RouteOrigin,
                entity.RouteDestination,
                entity.Date,
                entity.Time,
                entity.CompanyCuit
            });

            schedule.Property(entity => entity.Price).HasPrecision(10, 2);

            schedule.HasOne(entity => entity.Route)
                .WithMany()
                .HasForeignKey(entity => new { entity.RouteOrigin, entity.RouteDestination });

            schedule.HasOne(entity => entity.Company)
                .WithMany()
                .HasForeignKey(entity => entity.CompanyCuit);
        });

        modelBuilder.Entity<Booking>(booking =>
        {
            booking.HasKey(entity => new
            {
                entity.ScheduleRouteOrigin,
                entity.ScheduleRouteDestination,
                entity.ScheduleDate,
                entity.ScheduleTime,
                entity.ScheduleCompanyCuit,
                entity.UserDni
            });

            booking.HasOne(entity => entity.Schedule)
                .WithMany()
                .HasForeignKey(entity => new
                {
                    entity.ScheduleRouteOrigin,
                    entity.ScheduleRouteDestination,
                    entity.ScheduleDate,
                    entity.ScheduleTime,
                    entity.ScheduleCompanyCuit
                });

            booking.HasOne(entity => entity.User)
                .WithMany()
                .HasForeignKey(entity => entity.UserDni);
        });
    }
}
