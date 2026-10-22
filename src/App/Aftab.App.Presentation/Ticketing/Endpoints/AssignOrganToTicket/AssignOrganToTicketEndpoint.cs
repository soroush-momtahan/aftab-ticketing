using Aftab.App.Application.Ticketing.Features.Commands.AssignOrganToTicket;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.AssignOrganToTicket;

internal sealed class AssignOrganToTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tickets/{ticketId}/organs/{organId}", async (
            string ticketId,
            string organId,
            ISender sender) =>
        {
            Result result = await sender.Send(new AssignOrganToTicketCommand(ticketId, organId));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Ticketing)
        .RequireAuthorization(AppPermissions.Tickets.Update);
    }
}
