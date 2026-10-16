using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.People.Repository;

internal sealed class PeopleRepository(
    AftabDbContext dbContext) : IPeopleRepository
{
    public async Task CreateAsync(Person person, CancellationToken cancellationToken)
    {
        await dbContext.People.AddAsync(person, cancellationToken);
    }

    public async Task<Person?> FindByMobileAsync(string mobile, CancellationToken cancellationToken)
    {
        return await dbContext.People.SingleOrDefaultAsync(x => x.Mobile.Value == mobile, cancellationToken);
    }

    public async Task<Person?> FindByIdAsync(PersonId personId, CancellationToken cancellationToken)
    {
        return await dbContext.People.SingleOrDefaultAsync(x => x.Id == personId, cancellationToken);
    }
}
