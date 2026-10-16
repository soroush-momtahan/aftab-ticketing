using System.Security.Claims;
using Aftab.App.Domain.Identity.Enums;
using Aftab.App.Presentation.Ticketing.Policies.ReplyTicketOwnerShip;
using Aftab.App.Presentation.Ticketing.Policies.TicketOwnerShip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Aftab.App.Presentation.Extensions;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireCurrentUserOrRole(
        this RouteHandlerBuilder builder, 
        string routeParameterName = "personId",
        string bypassRole = "Agent")
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            ClaimsPrincipal user = context.HttpContext.User;
            
            string? currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                                    user.FindFirstValue("sub");
            string? role = user.FindFirstValue(ClaimTypes.Role);
            
            if (user.IsInRole(bypassRole))
            {
                return await next(context);
            }
            
            if (context.HttpContext.Request.RouteValues.TryGetValue(routeParameterName, out object? routeId) && routeId != null)
            {
                if (!string.Equals(currentUserId, routeId.ToString(), StringComparison.OrdinalIgnoreCase) || 
                    role != nameof(RoleType.Client))
                {
                    await next(context);
                }
                else
                {
                    TypedResults.Forbid();
                }
            }
            
            return await next(context);
        });
    }

    public static void ConfigureTicketPolicy(AuthorizationPolicyBuilder policyBuilder)
    {
        policyBuilder.Requirements.Add(new TicketOwnershipRequirement(bypassRole: "Agent"));
    }
    
    public static void ConfigureReplyTicketPolicy(AuthorizationPolicyBuilder policyBuilder)
    {
        policyBuilder.Requirements.Add(new ReplyTicketOwnerShipRequirement(bypassRole: "Agent"));
    }
}
