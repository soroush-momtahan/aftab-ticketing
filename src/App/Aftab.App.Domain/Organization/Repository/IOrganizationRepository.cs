using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.ValueObjects;

namespace Aftab.App.Domain.Organization.Repository;

public interface IOrganizationRepository
{
    Task AddAsync(Organ organ, CancellationToken cancellationToken);
    Task<Organ?> GetByIdAsync(OrganId organId, CancellationToken cancellationToken);
}
