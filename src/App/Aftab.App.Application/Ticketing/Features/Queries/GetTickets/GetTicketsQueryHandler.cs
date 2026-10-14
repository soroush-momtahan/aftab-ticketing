using Aftab.App.Application.Ticketing.Services;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Application.Pagination;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTickets;

internal sealed class GetTicketsQueryHandler(
    ITicketingViewService ticketingViewService) : IQueryHandler<GetTicketsQuery, PaginatedResult<GetTicketsQuery.Result>>
{
    public async Task<Result<PaginatedResult<GetTicketsQuery.Result>>> Handle(
        GetTicketsQuery query, CancellationToken cancellationToken)
    {
        List<TicketView> ticketViews = await ticketingViewService.GetAllAsync(
            query.SearchString,
            query.SortByPriority,
            query.FilterByDepartment,
            query.FilterByStatus,
            query.FilterByPersonId,
            query.PageIndex,
            query.PageSize,
            cancellationToken);

        var results = ticketViews
            .Select(GetTicketsQuery.ToResult)
            .ToList();

        return new PaginatedResult<GetTicketsQuery.Result>(
            query.PageIndex,
            query.PageSize,
            results.Count,
            results);
    }
    
}
