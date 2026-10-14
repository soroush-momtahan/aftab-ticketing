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

namespace Aftab.App.Presentation.Endpoints.Ticketing.GetTickets;

internal sealed class GetTicketsEndpoint : IEndpoint
{
    public record GetTicketsRequest(
        string? SearchString,
        bool SortByPriority,
        TicketDepartment? FilterByDepartment,
        TicketStatus? FilterByStatus,
        string? FilterByPersonId,
        int PageIndex = 1,
        int PageSize = 10);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Tickets", async (GetTicketsRequest request, ISender sender) =>
            {
                Result<PaginatedResult<GetTicketsQuery.Result>> result = await sender.Send(ToQuery(request));
                return result.Match(Results.Ok, ApiResults.Problem);
            })
        .WithTags(Tags.Ticketing);
    }

    private static GetTicketsQuery ToQuery(GetTicketsRequest request)
    {
        return new GetTicketsQuery(
            request.SearchString,
            request.SortByPriority,
            request.FilterByDepartment,
            request.FilterByStatus,
            request.FilterByPersonId,
            request.PageIndex,
            request.PageSize);
    }
}
