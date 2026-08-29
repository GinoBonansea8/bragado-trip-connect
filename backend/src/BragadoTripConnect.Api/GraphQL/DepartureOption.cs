namespace BragadoTripConnect.Api.GraphQL;

// What a traveller comparing a route on a given date needs to see. It carries
// the operator's name rather than its CUIT, which identifies the operator but
// means nothing to the person choosing a trip, and the schedule's id, which is
// what a booking will point at.
public record DepartureOption(
    int Id,
    string CompanyName,
    string Origin,
    string Destination,
    DateOnly Date,
    TimeOnly DepartureTime,
    int DurationMinutes,
    decimal Price);
