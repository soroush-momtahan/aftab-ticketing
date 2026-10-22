using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.Repository;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.Organization.Repository;

internal sealed class OrganizationRepository(
    AftabDbContext dbContext) : IOrganizationRepository
{
    public async Task AddAsync(Organ organ, CancellationToken cancellationToken)
    {
        await dbContext.Organs.AddAsync(organ, cancellationToken);
    }

    public async Task<Organ?> GetByIdAsync(OrganId organId, CancellationToken cancellationToken)
    {
        return await dbContext.Organs.SingleOrDefaultAsync(
            x => x.Id == organId, cancellationToken);
    }
}
