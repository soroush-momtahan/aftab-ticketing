using Aftab.App.Application.Ticketing.Features.Commands.UpdateTicketContent;
using Aftab.App.Presentation.Extensions;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Endpoints.Ticketing.UpdateTicketContent;

internal sealed class UpdateTicketContentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/people/{personId}/tickets/{ticketId}/content", async (
                string personId,
                string ticketId,
                string newContent,
                ISender sender) =>
            {
                Result result = await sender.Send(new UpdateTicketContentCommand(ticketId, newContent));
                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .RequireAuthorization()
            .RequireCurrentUserOrRole();
    }
}
