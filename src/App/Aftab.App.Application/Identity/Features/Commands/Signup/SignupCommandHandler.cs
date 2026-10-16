using Aftab.App.Application.Abstraction;
using Aftab.App.Application.Identity.Services;
using Aftab.App.Domain.Identity.Errors;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Application.Caching;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Exceptions;
using Aftab.Shared.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Application.Identity.Features.Commands.Signup;

public sealed class SignupCommandHandler(
    IPeopleRepository peopleRepository,
    ICacheService cacheService,
    IIdentityService identityService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : ICommandHandler<SignupCommand, SignupCommand.Result>
{
    public async Task<Result<SignupCommand.Result>> Handle(SignupCommand command, CancellationToken cancellationToken)
    {
        string? mobile = await cacheService.GetAsync<string>($"{command.TempToken}", cancellationToken);
        if (mobile is null)
        {
            return Result.Failure<SignupCommand.Result>(IdentityErrors.TempVerifyTokenInvalid);
        }

        Person? user = await peopleRepository.FindByMobileAsync(mobile, cancellationToken);
        if (user is not null)
        {
            return Result.Failure<SignupCommand.Result>(PeopleErrors.DuplicateMobile(mobile));
        }
        
        Result<FullName> fullNameOrError = FullName.From(command.FirstName, command.LastName);
        if (fullNameOrError.IsFailure)
        {
            return Result.Failure<SignupCommand.Result>(fullNameOrError.Error);
        }
        
        Result<Mobile> mobileOrError = Mobile.From(mobile);
        if (mobileOrError.IsFailure)
        {
            return Result.Failure<SignupCommand.Result>(mobileOrError.Error);
        }
        
        var person = Person.Create(fullNameOrError.Value, mobileOrError.Value);
        
        Result<IdentityUser> identityUserOrErrors = await identityService.SignupWithMobileAsync(person.Id, mobile, command.Password);
        if (identityUserOrErrors.IsFailure)
        {
            throw new AftabException(nameof(identityService.SignupWithMobileAsync), identityUserOrErrors.Error);
        }
        
        await peopleRepository.CreateAsync(person, cancellationToken);

        string token = null;

        await unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        
            token = await tokenService.GenerateTokenAsync(identityUserOrErrors.Value);

            
        }, cancellationToken);

        return token is null ? throw new AftabException(nameof(SignupCommandHandler)) : new SignupCommand.Result(token);
    }
}
