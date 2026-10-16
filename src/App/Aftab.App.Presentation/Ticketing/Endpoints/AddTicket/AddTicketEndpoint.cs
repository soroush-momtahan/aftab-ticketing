using System.Security.Claims;
using Aftab.App.Application.Ticketing.Features.Commands.AddTicket;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Domain.Exceptions;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.AddTicket;

internal sealed class AddTicketEndpoint : IEndpoint
{
    public sealed record AddTicketRequest(
        string Title,
        string Content,
        TicketPriority Priority,
        TicketDepartment Department);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets", async (
                AddTicketRequest request,
                ClaimsPrincipal person,
                ISender sender) =>
            {
                string? currentPersonId = person.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                                          person.FindFirstValue("sub");
                if (currentPersonId is null)
                {
                    throw new AftabException(nameof(AddTicketEndpoint), PeopleErrors.PersonNotFound);
                }
                Result result = await sender.Send(ToCommand(request, currentPersonId));
                return result.Match(() => Results.Ok(), ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization();
    }

    private static AddTicketCommand ToCommand(AddTicketRequest request, string personId)
    {
        return new AddTicketCommand(
            personId, 
            request.Title, 
            request.Content, 
            request.Priority, 
            request.Department);
    }
}
