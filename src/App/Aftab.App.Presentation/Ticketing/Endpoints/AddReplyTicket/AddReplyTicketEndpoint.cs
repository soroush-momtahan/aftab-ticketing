using System.Security.Claims;
using Aftab.App.Application.Ticketing.Features.Commands.AddReplyTicket;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.AddReplyTicket;

internal sealed class AddReplyTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{ticketId}/reply", async (
            string ticketId, 
            string content,
            ClaimsPrincipal person,
            ISender sender) =>
        {
            string currentPersonId = person.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                                      person.FindFirstValue("sub");
            Result result = await sender.Send(new AddReplyTicketCommand(ticketId, currentPersonId!, content));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Ticketing)
        .RequireAuthorization(AppPolices.Ticketing.CanManageTicket);
    }
}
