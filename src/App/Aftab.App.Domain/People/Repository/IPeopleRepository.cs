using Aftab.App.Domain.People.Models;

namespace Aftab.App.Domain.People.Repository;

public interface IPeopleRepository
{
    Task CreateAsync(Person person, CancellationToken cancellationToken);
    Task<Person?> FindByMobileAsync(string mobile, CancellationToken cancellationToken);
}

