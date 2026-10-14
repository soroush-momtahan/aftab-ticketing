using Aftab.App.Application.Ticketing.Features.Queries.GetTickets;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.App.Presentation.Extensions;
using Aftab.Shared.Application.Pagination;
using Aftab.Shared.Domain.Results;
using Aftab.Shared.Presentation.ApiResults;
using Aftab.Shared.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aftab.App.Presentation.Endpoints.Ticketing.GetUserTickets;

internal sealed class GetUserTicketsEndpoint : IEndpoint
{
    public record GetUserTicketsRequest(
        string? SearchString,
        bool SortByPriority,
        TicketDepartment? FilterByDepartment,
        TicketStatus? FilterByStatus,
        int PageIndex = 1,
        int PageSize = 10);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/person/{personId}/tickets", async (
                string personId, 
                GetUserTicketsRequest request, 
                ISender sender) =>
            {
                Result<PaginatedResult<GetTicketsQuery.Result>> result = await sender.Send(
                    ToQuery(request, personId));
                return result.Match(Results.Ok, ApiResults.Problem);
            })
        .WithTags(Tags.Ticketing)
        .RequireAuthorization()
        .RequireCurrentUserOrRole();
    }

    private static GetTicketsQuery ToQuery(GetUserTicketsRequest request, string personId)
    {
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
