using Microsoft.AspNetCore.Identity;

namespace Aftab.App.Application.Identity.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(IdentityUser user);
}
