using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.ValueObjects;

namespace Aftab.App.Domain.People.Repository;

public interface IPeopleRepository
{
    Task CreateAsync(Person person, CancellationToken cancellationToken);
    Task<Person?> FindByMobileAsync(string mobile, CancellationToken cancellationToken);
    Task<Person?> FindByIdAsync(PersonId personId, CancellationToken cancellationToken);
}

