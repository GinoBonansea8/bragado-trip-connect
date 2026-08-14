using BragadoTripConnect.Api.Data;
using Microsoft.EntityFrameworkCore;

// ASP.NET Core has its own Route type for URL routing, and the Web SDK imports
// it implicitly, so the name has to be pinned to ours.
using Route = BragadoTripConnect.Api.Data.Route;

namespace BragadoTripConnect.Api.GraphQL;

public class Query
{
    public async Task<HealthCheck?> GetHealthAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.HealthChecks
            .OrderByDescending(h => h.CheckedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Company>> GetCompaniesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Companies
            .OrderBy(company => company.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Route>> GetRoutesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Routes
            .OrderBy(route => route.Origin)
            .ThenBy(route => route.Destination)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DepartureOption>> SearchDeparturesAsync(
        string origin,
        string destination,
        DateOnly date,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Schedules
            .Where(schedule => schedule.RouteOrigin == origin
                && schedule.RouteDestination == destination
                && schedule.Date == date)
            .OrderBy(schedule => schedule.Time)
            .Select(schedule => new DepartureOption(
                schedule.CompanyCuit,
                schedule.Company.Name,
                schedule.RouteOrigin,
                schedule.RouteDestination,
                schedule.Date,
                schedule.Time,
                schedule.DurationMinutes,
                schedule.Price))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Schedule>> GetDeparturesByCompanyAsync(
        string companyCuit,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Schedules
            .Where(schedule => schedule.CompanyCuit == companyCuit)
            .OrderBy(schedule => schedule.Date)
            .ThenBy(schedule => schedule.Time)
            .ToListAsync(cancellationToken);
    }
}
