namespace BragadoTripConnect.Api.Data;

public class HealthCheck
{
    public int Id { get; set; }

    public string Status { get; set; } = "ok";

    public DateTimeOffset CheckedAt { get; set; }
}
