using System.Security.Claims;
using Aftab.App.Application.Ticketing.Features.Queries.GetTickets;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Application.Pagination;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Ticketing.Endpoints.GetTickets;

internal sealed class GetTicketsEndpoint : IEndpoint
{
    public sealed record GetTicketsRequest(
        string? SearchString,
        bool SortByPriority,
        TicketDepartment? FilterByDepartment,
        TicketStatus? FilterByStatus,
        string? FilterByPersonId,
        int PageIndex = 1,
        int PageSize = 10);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Tickets", async (
                [AsParameters] GetTicketsRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                bool agent = user.IsInRole("Agent");
                string? currentPersonId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                                        user.FindFirstValue("sub");
                Result<PaginatedResult<GetTicketsQuery.Result>> result = await sender.Send(ToQuery(
                    request, agent, currentPersonId));
                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization();
    }

    private static GetTicketsQuery ToQuery(GetTicketsRequest request, bool isAgent, string? currentPersonId)
    {
        string personId = isAgent ? request.FilterByPersonId : currentPersonId;
        Console.WriteLine(personId);
        return new GetTicketsQuery(
            request.SearchString,
            request.SortByPriority,
            request.FilterByDepartment,
            request.FilterByStatus,
            personId,
            request.PageIndex,
            request.PageSize);
    }
}
