using Aftab.App.Application.Identity.Services;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Application.Identity.Features.Commands.SignInWithPassword;

public sealed class SignInWithPasswordCommandHandler(
    IIdentityService identityService,
    ITokenService tokenService) : ICommandHandler<SignInWithPasswordCommand,  SignInWithPasswordCommand.Result>
{
    public async Task<Result<SignInWithPasswordCommand.Result>> Handle(SignInWithPasswordCommand command, CancellationToken cancellationToken)
    {
        Result<IdentityUser> signinOrError = await identityService.SigninByMobileAsync(command.Mobile, command.Password);
        if (signinOrError.IsFailure)
        {
            return Result.Failure<SignInWithPasswordCommand.Result>(signinOrError.Error);
        }

        string token = await tokenService.GenerateTokenAsync(signinOrError.Value);

        return new SignInWithPasswordCommand.Result(token);
    }
}
