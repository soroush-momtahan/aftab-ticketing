using Aftab.App.Application.Ticketing.Features.Commands.UpdateReplyTicketContent;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Endpoints.Ticketing.UpdateReplyTicketContent;

internal sealed class UpdateReplyTicketContentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/people/{personId}/tickets/{ticketId}/ReplyTickets/{replyTicketId}/content", async (
            string personId,
            string ticketId,
            string replyTicketId,
            string newContent,
            ISender sender) =>
        {
            Result result = await sender.Send(
                new UpdateReplyTicketContentCommand(ticketId, replyTicketId, newContent));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        });
    }
}
