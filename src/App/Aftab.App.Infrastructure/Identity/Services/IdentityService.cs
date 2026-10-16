using Aftab.App.Application.Identity.Services;
using Aftab.App.Domain.Identity.Errors;
using Aftab.App.Domain.People.Errors;
using Aftab.Shared.Domain.Errors;
using Aftab.Shared.Domain.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.Identity.Services;

public class IdentityService(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager) : IIdentityService
{
    public async Task<IdentityUser?> FindByMobileAsync(string mobile)
    {
        return await userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == mobile);
    }

    public async Task<Result<IdentityUser>> SignupWithMobileAsync(string id, string mobile, string password)
    {
        var identityUser = new IdentityUser
        {
            Id = id,
            UserName = mobile,
            PhoneNumber = mobile,
            PhoneNumberConfirmed = true
        };
        List<Error> errors = [];
        IdentityResult identityUserOrError = await userManager.CreateAsync(identityUser, password);
        if (!identityUserOrError.Succeeded)
        {
            foreach (IdentityError err in identityUserOrError.Errors)
            {
                errors.Add(new Error(err.Code, err.Description, ErrorType.Validation));
            }

            return Result.Failure<IdentityUser>(Error.Combine(errors.ToArray()));
        }
        
        await signInManager.PasswordSignInAsync(identityUser, password, true, false);
        
        IdentityUser user = await userManager.FindByNameAsync(mobile);
        
        return user;
    }

    public async Task<Result<IdentityUser>> SigninByMobileAsync(string mobile, string password)
    { 
        IdentityUser? user = await userManager.FindByNameAsync(mobile);
        if (user is null)
        {
            return Result.Failure<IdentityUser>(PeopleErrors.PersonNotFound);
        }

        SignInResult result = await signInManager.PasswordSignInAsync(
            mobile, password, true, true);
        if (!result.Succeeded)
        {
            return Result.Failure<IdentityUser>(IdentityErrors.PasswordIsNotValid);
        }

        return user;
    }
}
