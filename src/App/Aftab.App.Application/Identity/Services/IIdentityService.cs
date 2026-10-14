using Aftab.Shared.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Application.Identity.Services;

public interface IIdentityService
{
    Task<IdentityUser?> FindByMobileAsync(string mobile);
    Task<Result<IdentityUser>> SignupWithMobileAsync(string mobile, string password);
    Task<Result<IdentityUser>> SigninByMobileAsync(string mobile, string password);
}
