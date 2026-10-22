using System.Security.Claims;
using Aftab.App.Application.Ticketing.Features.Queries.GetTicketOwnerId;
using Aftab.App.Domain.Identity.Enums;
using Aftab.Shared.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Aftab.App.Presentation.Ticketing.Policies.TicketOwnerShip;

public class TicketOwnershipHandler(ISender sender, IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<TicketOwnershipRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TicketOwnershipRequirement requirement)
    {
        // اگر نقش Bypass داشته باشد ← دسترسی کامل
        if (context.User.IsInRole(requirement.BypassRole) ||
            context.User.IsInRole(nameof(RoleType.Admin)) ||
            context.User.IsInRole(nameof(RoleType.Agent)))
        {
            context.Succeed(requirement);
            return;
        }

        // گرفتن ticketId از Route
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null ||
            !httpContext.Request.RouteValues.TryGetValue("ticketId", out object? ticketIdObj) ||
            ticketIdObj is null)
        {
            return; // Fail
        }

        string ticketId = ticketIdObj.ToString()!;
        string? currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(currentUserId))
        {
            return;
        }
        
        Result<GetTicketOwnerIdQuery.Result> ticketOrError = await sender.Send(new GetTicketOwnerIdQuery(ticketId));

        if (ticketOrError.IsFailure)
        {
            return;
        }

        if (ticketOrError.Value.OwnerId == currentUserId)
        {
            context.Succeed(requirement);
        }
    }
}
