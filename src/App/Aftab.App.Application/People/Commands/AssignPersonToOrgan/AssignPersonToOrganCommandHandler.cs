using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Organization.Errors;
using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.Repository;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.People.Commands.AssignPersonToOrgan;

internal sealed class AssignPersonToOrganCommandHandler(
    IPeopleRepository peopleRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignPersonToOrganCommand>
{
    public async Task<Result> Handle(
        AssignPersonToOrganCommand command, CancellationToken cancellationToken)
    {
        Result<PersonId> personIdOrError = PrefixedGuidV3.From<PersonId>(command.PersonId);
        if (personIdOrError.IsFailure)
        {
            return Result.Failure(personIdOrError.Error);
        }

        Result<OrganId> organIdOrError = PrefixedGuidV3.From<OrganId>(command.OrganId);
        if (organIdOrError.IsFailure)
        {
            return Result.Failure(organIdOrError.Error);
        }

        Person? person = await peopleRepository.FindByIdAsync(personIdOrError.Value, cancellationToken);
        if (person is null)
        {
            return Result.Failure(PeopleErrors.PersonNotFound);
        }

        Organ? organ = await organizationRepository.GetByIdAsync(organIdOrError.Value, cancellationToken);
        if (organ is null)
        {
            return Result.Failure(OrganizationErrors.NotFound);
        }
        
        person.AssignToOrgan(organIdOrError.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
