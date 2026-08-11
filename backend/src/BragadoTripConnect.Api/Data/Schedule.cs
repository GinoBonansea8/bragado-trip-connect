namespace BragadoTripConnect.Api.Data;

// Route and Company below are hidden from the GraphQL schema: they are only
// loaded when a query asks for them, and everything the API exposes today is
// already on the schedule itself.

public class Schedule
{
    public string RouteOrigin { get; set; } = null!;

    public string RouteDestination { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string CompanyCuit { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    [GraphQLIgnore]
    public Route Route { get; set; } = null!;

    [GraphQLIgnore]
    public Company Company { get; set; } = null!;
}
