using System.Security.Claims;
using Aftab.Shared.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Aftab.Shared.Infrastructure.Security;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return;
        }
        
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IPermissionService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        IEnumerable<string> roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        IReadOnlySet<string> granted = await permissionService.GetForRolesAsync(roles);

        if (granted.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
