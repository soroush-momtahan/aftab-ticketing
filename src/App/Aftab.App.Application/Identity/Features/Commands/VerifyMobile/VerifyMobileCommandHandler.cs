using Aftab.App.Application.Identity.Services;
using Aftab.App.Domain.Identity.Errors;
using Aftab.Shared.Application.Caching;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Application.Identity.Features.Commands.VerifyMobile;

public class VerifyMobileCommandHandler(
    ICacheService cacheService,
    IIdentityService identityService,
    ITokenService tokenService) : ICommandHandler<VerifyMobileCommand, VerifyMobileResult>
{
    public async Task<Result<VerifyMobileResult>> Handle(VerifyMobileCommand command, CancellationToken cancellationToken)
    {
        string? code = await cacheService.GetAsync<string?>($"Otp:{command.Mobile}", cancellationToken);

        if (code is null || code != command.Code)
        {
            return Result.Failure<VerifyMobileResult>(IdentityErrors.VerifyCodeIsNotValid);
        }
        
        await cacheService.RemoveAsync($"Otp:{command.Mobile}", cancellationToken);
        
        IdentityUser? identityUser = await identityService.FindByMobileAsync(command.Mobile);
        
        bool isNewUser = identityUser == null;

        string token;

        if (isNewUser)
        {
            token = $"Verified:{Guid.CreateVersion7()}";
        
            await cacheService.SetAsync(
                token, 
                command.Mobile,
                new TimeSpan(0,5,0), cancellationToken);
        }
        else
        {
            token = await tokenService.GenerateTokenAsync(identityUser!);
        }

        

        
        
        return new VerifyMobileResult(isNewUser, token);
    }
}
