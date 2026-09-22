namespace BragadoTripConnect.Api.GraphQL;

// A departure as the operator that published it sees it: the same trip as a
// DepartureOption, minus the company name, which is always their own.
public record CompanyDeparture(
    int Id,
    string Origin,
    string Destination,
    DateOnly Date,
    TimeOnly DepartureTime,
    int DurationMinutes,
    decimal Price);
