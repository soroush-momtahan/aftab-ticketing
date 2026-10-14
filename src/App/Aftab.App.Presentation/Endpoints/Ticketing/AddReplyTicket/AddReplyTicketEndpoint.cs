using Aftab.App.Application.Ticketing.Features.Commands.AddReplyTicket;
using Aftab.App.Presentation.Extensions;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Endpoints.Ticketing.AddReplyTicket;

internal sealed class AddReplyTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{ticketId}/replier/{replierId}", async (
            string ticketId, string replierId, string content, ISender sender) =>
        {
            Result result = await sender.Send(new AddReplyTicketCommand(ticketId, replierId, content));
            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Ticketing)
        .RequireAuthorization()
        .RequireCurrentUserOrRole("replierId");
    }
}
