using Aftab.App.Application.Identity.Features.Commands.ChangeRole;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Events;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;
using MediatR;

namespace Aftab.App.Application.People.EventHandlers.PersonAssignedToOrgan;

internal sealed class ChangePersonRoleOnPersonAssignedToOrgan(
    ISender sender,
    IPeopleRepository peopleRepository) : DomainEventHandler<PersonAssignedToOrganEvent>
{
    public override async Task Handle(PersonAssignedToOrganEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<PersonId> personIdOrError = PrefixedGuidV3.From<PersonId>(domainEvent.PersonId);
        if (personIdOrError.IsFailure)
        {
            throw new AftabException(
                nameof(ChangePersonRoleOnPersonAssignedToOrgan),
                personIdOrError.Error);
        }
        
        Person? person = await peopleRepository.FindByIdAsync(personIdOrError.Value, cancellationToken);
        if (person is null)
        {
            throw new AftabException(
                nameof(ChangePersonRoleOnPersonAssignedToOrgan),
                PeopleErrors.PersonNotFound);
        }
        Result resultOrError = await sender.Send(new ChangeRoleCommand(
            domainEvent.PersonId,
            person.RoleType), cancellationToken);
        if (resultOrError.IsFailure)
        {
            throw new AftabException(
                nameof(ChangePersonRoleOnPersonAssignedToOrgan),
                resultOrError.Error);
        }
    }
}
