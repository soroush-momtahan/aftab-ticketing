using Aftab.App.Application.Ticketing.Features.Commands.SoftDeleteTicket;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.SoftDeleteTicket;

internal sealed class SoftDeleteTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/tickets/{ticketId}", async (
                string ticketId,
                ISender sender) =>
            {
                Result result = await sender.Send(new SoftDeleteTicketCommand(ticketId));
                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization(AppPolices.Ticketing.CanManageTicket);
    }
}
