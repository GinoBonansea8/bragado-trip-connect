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
