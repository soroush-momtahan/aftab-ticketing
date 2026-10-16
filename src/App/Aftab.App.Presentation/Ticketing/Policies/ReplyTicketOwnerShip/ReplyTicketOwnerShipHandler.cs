using System.Security.Claims;
using Aftab.App.Application.Ticketing.Features.Queries.GetReplyTicketOwnerId;
using Aftab.App.Application.Ticketing.Features.Queries.GetTicketOwnerId;
using Aftab.App.Domain.Identity.Enums;
using Aftab.Shared.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Aftab.App.Presentation.Ticketing.Policies.ReplyTicketOwnerShip;

public class ReplyTicketOwnerShipHandler(ISender sender, IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<ReplyTicketOwnerShipRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ReplyTicketOwnerShipRequirement requirement)
    {
        // اگر نقش Bypass داشته باشد ← دسترسی کامل
        if (context.User.IsInRole(requirement.BypassRole) || 
            context.User.IsInRole(nameof(RoleType.Agent)))
        {
            context.Succeed(requirement);
            return;
        }

        // گرفتن ticketId از Route
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null ||
            !httpContext.Request.RouteValues.TryGetValue("ticketId", out object? ticketIdObj) ||
            !httpContext.Request.RouteValues.TryGetValue("replyTicketId", out object? replyTicketIdObj) ||
            ticketIdObj is null ||
            replyTicketIdObj is null)
        {
            return; // Fail
        }

        string ticketId = ticketIdObj.ToString()!;
        string replyTicketId = replyTicketIdObj.ToString()!;
        string? currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(currentUserId))
        {
            return;
        }
        
        Result<GetReplyTicketOwnerIdQuery.Result> replyTicketOrError = await sender.Send(new GetReplyTicketOwnerIdQuery(replyTicketId));
        if (replyTicketOrError.IsFailure)
        {
            return;
        }
        if (replyTicketOrError.Value.OwnerId != currentUserId)
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
