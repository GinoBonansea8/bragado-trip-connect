namespace BragadoTripConnect.Api.Data;

public class Route
{
    public int Id { get; set; }

    public string Origin { get; set; } = null!;

    public string Destination { get; set; } = null!;
}
