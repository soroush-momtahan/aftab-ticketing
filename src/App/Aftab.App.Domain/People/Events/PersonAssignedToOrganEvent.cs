using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Domain.Events;

namespace Aftab.App.Domain.People.Events;

public class PersonAssignedToOrganEvent(PersonId personId) : DomainEvent
{
    public PersonId PersonId { get; } = personId;
}
