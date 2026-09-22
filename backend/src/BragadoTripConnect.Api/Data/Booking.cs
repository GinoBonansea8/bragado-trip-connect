namespace BragadoTripConnect.Api.Data;

public class Booking
{
    public int Id { get; set; }

    public int ScheduleId { get; set; }

    public int UserId { get; set; }

    [GraphQLIgnore]
    public Schedule Schedule { get; set; } = null!;

    [GraphQLIgnore]
    public User User { get; set; } = null!;
}
