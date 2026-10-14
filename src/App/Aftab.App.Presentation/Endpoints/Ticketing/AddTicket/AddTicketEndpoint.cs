using Aftab.App.Application.Ticketing.Features.Commands.AddTicket;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Endpoints.Ticketing.AddTicket;

internal sealed class AddTicketEndpoint : IEndpoint
{
    public record AddTicketRequest(
        string PersonId,
        string Title,
        string Content,
        TicketPriority Priority,
        TicketDepartment Department);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets", async (
                AddTicketRequest request, ISender sender) =>
            {
                Result result = await sender.Send(ToCommand(request));
                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization();
    }

    private static AddTicketCommand ToCommand(AddTicketRequest request)
    {
        return new AddTicketCommand(
            request.PersonId, 
            request.Title, 
            request.Content, 
            request.Priority, 
            request.Department);
    }
}
