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
    public async Task<Schedule> PublishDepartureAsync(
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

        var companyExists = await dbContext.Companies
            .AnyAsync(company => company.Cuit == input.CompanyCuit, cancellationToken);

        if (!companyExists)
        {
            throw new GraphQLException($"No hay ninguna empresa registrada con el CUIT {input.CompanyCuit}.");
        }

        var routeExists = await dbContext.Routes
            .AnyAsync(route => route.Origin == input.Origin && route.Destination == input.Destination, cancellationToken);

        if (!routeExists)
        {
            throw new GraphQLException($"No hay ninguna ruta registrada de {input.Origin} a {input.Destination}.");
        }

        var alreadyPublished = await dbContext.Schedules.AnyAsync(
            schedule => schedule.CompanyCuit == input.CompanyCuit
                && schedule.RouteOrigin == input.Origin
                && schedule.RouteDestination == input.Destination
                && schedule.Date == input.Date
                && schedule.Time == input.DepartureTime,
            cancellationToken);

        if (alreadyPublished)
        {
            throw new GraphQLException("Esta empresa ya publicó una salida en esa ruta, fecha y horario.");
        }

        var departure = new Schedule
        {
            CompanyCuit = input.CompanyCuit,
            RouteOrigin = input.Origin,
            RouteDestination = input.Destination,
            Date = input.Date,
            Time = input.DepartureTime,
            DurationMinutes = input.DurationMinutes,
            Price = input.Price
        };

        dbContext.Schedules.Add(departure);
        await dbContext.SaveChangesAsync(cancellationToken);

        return departure;
    }
}
