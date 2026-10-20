using System.Security.Claims;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Identity.GetMe;

internal sealed class GetMeEndpoint : IEndpoint
{
    public sealed record GetMeResponse(string Id, string? UserName, string[] Roles, string[] Permissions);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/me", async (
            ClaimsPrincipal user,
            IPermissionService permissionService,
            CancellationToken ct) =>
        {
            string[] roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
            IReadOnlySet<string> permissions = await permissionService.GetForRolesAsync(roles, ct);
            return new GetMeResponse(
                user.FindFirstValue(ClaimTypes.NameIdentifier)!,
                user.Identity!.Name,
                roles,
                [.. permissions.Order()]);
        })
        .WithTags(Tags.Identity)
        .RequireAuthorization();
    }
}
