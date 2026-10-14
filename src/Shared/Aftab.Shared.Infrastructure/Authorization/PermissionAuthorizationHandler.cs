using System.Security.Claims;
using Aftab.Shared.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Aftab.Shared.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        string? memberId = context.User.Claims.FirstOrDefault(
            x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(memberId, out Guid parsedMemberId))
        {
            return;
        }
        
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IPermissionService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        HashSet<string> permissions = await permissionService.GetPermissionsAsync(parsedMemberId);
        
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
