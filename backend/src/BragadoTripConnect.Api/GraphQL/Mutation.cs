using BragadoTripConnect.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BragadoTripConnect.Api.GraphQL;

public record PublishDepartureInput(
    string CompanyCuit,
    string Origin,
    string Destination,
    DateOnly Date,
    TimeOnly DepartureTime,
    int DurationMinutes,
    decimal Price);

// The messages below are written in Spanish, unlike the rest of the code: they
// travel to the browser and are shown to the operator word for word.
public class Mutation
{
    public async Task<CompanyDeparture> PublishDepartureAsync(
        PublishDepartureInput input,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (input.DurationMinutes <= 0)
        {
            throw new GraphQLException("El viaje tiene que durar más de cero minutos.");
        }

        if (input.Price < 0)
        {
            throw new GraphQLException("El precio no puede ser negativo.");
        }

        // The company and the route are looked up rather than just checked for:
        // the schedule points at them by id now, so the rows themselves are
        // what the departure is built from.
        var company = await dbContext.Companies
            .FirstOrDefaultAsync(company => company.Cuit == input.CompanyCuit, cancellationToken);

        if (company is null)
        {
            throw new GraphQLException($"No hay ninguna empresa registrada con el CUIT {input.CompanyCuit}.");
        }

        var route = await dbContext.Routes.FirstOrDefaultAsync(
            route => route.Origin == input.Origin && route.Destination == input.Destination,
            cancellationToken);

        if (route is null)
        {
            throw new GraphQLException($"No hay ninguna ruta registrada de {input.Origin} a {input.Destination}.");
        }

        // The database rejects this too, through the unique index on the
        // schedule. Checking first turns that into a sentence the operator can
        // read instead of a constraint violation.
        var alreadyPublished = await dbContext.Schedules.AnyAsync(
            schedule => schedule.CompanyId == company.Id
                && schedule.RouteId == route.Id
                && schedule.Date == input.Date
                && schedule.Time == input.DepartureTime,
            cancellationToken);

        if (alreadyPublished)
        {
            throw new GraphQLException("Esta empresa ya publicó una salida en esa ruta, fecha y horario.");
        }

        var departure = new Schedule
        {
            CompanyId = company.Id,
            RouteId = route.Id,
            Date = input.Date,
            Time = input.DepartureTime,
            DurationMinutes = input.DurationMinutes,
            Price = input.Price
        };

        dbContext.Schedules.Add(departure);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompanyDeparture(
            departure.Id,
            route.Origin,
            route.Destination,
            departure.Date,
            departure.Time,
            departure.DurationMinutes,
            departure.Price);
    }
}
