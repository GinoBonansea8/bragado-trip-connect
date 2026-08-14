namespace BragadoTripConnect.Api.GraphQL;

// What a traveller comparing a route on a given date needs to see. It carries
// the operator's name rather than only its CUIT, which identifies the operator
// but means nothing to the person choosing a trip.
public record DepartureOption(
    string CompanyCuit,
    string CompanyName,
    string Origin,
    string Destination,
    DateOnly Date,
    TimeOnly DepartureTime,
    int DurationMinutes,
    decimal Price);
