using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Application.Pagination;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTickets;

internal sealed class GetTicketsQueryHandler(
    ITicketingQueryService ticketingQueryService) : IQueryHandler<GetTicketsQuery, PaginatedResult<GetTicketsQuery.Result>>
{
    public async Task<Result<PaginatedResult<GetTicketsQuery.Result>>> Handle(
        GetTicketsQuery query, CancellationToken cancellationToken)
    {
        PersonId? personId = null;
        if (query.FilterByPersonId != null)
        {
            Result<PersonId> personIdOrError = PrefixedGuidV3.From<PersonId>(query.FilterByPersonId);
            if (personIdOrError.IsFailure)
            {
                return Result.Failure<PaginatedResult<GetTicketsQuery.Result>>(personIdOrError.Error);
            }
            personId = personIdOrError.Value;
        }
        
        List<TicketView> ticketViews = await ticketingQueryService.GetAllAsync(
            query.SearchString,
            query.SortByPriority,
            query.FilterByDepartment,
            query.FilterByStatus,
            personId,
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
