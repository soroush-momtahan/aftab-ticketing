using Aftab.App.Application.Identity.Services;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Application.Identity.Features.Commands.ChangeRole;

internal sealed class ChangeRoleCommandHandler(
    IIdentityService identityService,
    IPeopleRepository peopleRepository,
    UserManager<IdentityUser> userManager) : ICommandHandler<ChangeRoleCommand>
{
    public async Task<Result> Handle(
        ChangeRoleCommand command, CancellationToken cancellationToken)
    {
        Result<PersonId> personIdOrError = PrefixedGuidV3.From<PersonId>(command.PersonId);
        if (personIdOrError.IsFailure)
        {
            return Result.Failure(personIdOrError.Error);
        }
        
        Person? person = await peopleRepository.FindByIdAsync(personIdOrError.Value, cancellationToken);
        if (person is null)
        {
            return Result.Failure(personIdOrError.Error);
        }

        string mobile = person.Mobile.Value;
        
        IdentityUser? identityUser = await identityService.FindByMobileAsync(mobile);
        if (identityUser is null)
        {
            return Result.Failure<Result>(PeopleErrors.PersonNotFound);
        }
        
        await userManager.AddToRoleAsync(identityUser, command.RoleType.ToString());
        return Result.Success();
    }
}
