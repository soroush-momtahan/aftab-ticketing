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
        string? FilterByOrganId,
        int PageIndex = 1,
        int PageSize = 10);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Tickets", async (
                [AsParameters] GetTicketsRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                bool isAdmin = user.IsInRole("Admin");
                bool isAgent = user.IsInRole("Agent");
                string? currentPersonId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                                        user.FindFirstValue("sub");
                Result<PaginatedResult<GetTicketsQuery.Result>> result = await sender.Send(ToQuery(
                    request, 
                    isAdmin,
                    isAgent,
                    currentPersonId));
                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Ticketing)
            .RequireAuthorization();
    }

    private static GetTicketsQuery ToQuery(
        GetTicketsRequest request,
        bool isAdmin,
        bool isAgent,
        string? currentPersonId)
    {
#pragma warning disable S125
        // string personId = isAdmin || isAgent ? request.FilterByPersonId : currentPersonId;
        // string? filteredByOrganId = isAdmin ? request.FilterByOrganId : null;
        
        bool onlyVisibleReplies = !(isAdmin || isAgent);
        return new GetTicketsQuery(
            currentPersonId!,
            request.SearchString,
            request.SortByPriority,
            onlyVisibleReplies,
            request.FilterByDepartment,
            request.FilterByStatus,
            request.FilterByPersonId,
            request.FilterByOrganId,
            request.PageIndex,
            request.PageSize);
    }
}
