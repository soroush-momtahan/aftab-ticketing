using Aftab.App.Application.Ticketing.Features.Commands.SoftDeleteReplyTicket;
using Aftab.App.Presentation.Extensions;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Endpoints.Ticketing.SoftDeleteReplyTicket;

internal sealed class SoftDeleteReplyTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/people/{personId}/tickets/{ticketId}/Replies/{replyTicketId}", async (
                string personId,
                string ticketId,
                string replyTicketId,
                ISender sender) =>
            {
                Result result = await sender.Send(new SoftDeleteReplyTicketCommand(ticketId, replyTicketId));
                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization()
            .RequireCurrentUserOrRole();
    }
}
