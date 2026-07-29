using BragadoTripConnect.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BragadoTripConnect.Api.GraphQL;

public class Query
{
    public async Task<HealthCheck?> GetHealthAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.HealthChecks
            .OrderByDescending(h => h.CheckedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
