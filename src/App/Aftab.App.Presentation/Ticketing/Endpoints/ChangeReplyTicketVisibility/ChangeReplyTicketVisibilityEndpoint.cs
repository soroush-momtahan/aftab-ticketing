using Aftab.App.Application.Ticketing.Features.Commands.ChangeReplyTicketVisibility;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.ChangeReplyTicketVisibility;

internal sealed class ChangeReplyTicketVisibilityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tickets/{ticketId}/replies/{replyTicketId}/visibility", async (
            string ticketId,
            string replyTicketId,
            bool visibility,
            ISender sender) =>
        {
            Result result = await sender.Send(
                new ChangeReplyTicketVisibilityCommand(
                    ticketId, replyTicketId, visibility));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Ticketing)
        .RequireAuthorization(AppPermissions.Tickets.Update);
    }
}
