namespace BragadoTripConnect.Api.Data;

public class Schedule
{
    public string RouteOrigin { get; set; } = null!;

    public string RouteDestination { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string CompanyCuit { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public Route Route { get; set; } = null!;

    public Company Company { get; set; } = null!;
}
