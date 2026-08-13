namespace BragadoTripConnect.Api.Data;

public class Booking
{
    public string ScheduleRouteOrigin { get; set; } = null!;

    public string ScheduleRouteDestination { get; set; } = null!;

    public DateOnly ScheduleDate { get; set; }

    public TimeOnly ScheduleTime { get; set; }

    public string ScheduleCompanyCuit { get; set; } = null!;

    public string UserDni { get; set; } = null!;

    [GraphQLIgnore]
    public Schedule Schedule { get; set; } = null!;

    [GraphQLIgnore]
    public User User { get; set; } = null!;
}
