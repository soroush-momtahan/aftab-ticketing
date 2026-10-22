using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Identity.Enums;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Application.Pagination;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTickets;

internal sealed class GetTicketsQueryHandler(
    ITicketingQueryService ticketingQueryService,
    IPeopleRepository peopleRepository) : IQueryHandler<GetTicketsQuery, PaginatedResult<GetTicketsQuery.Result>>
{
    public async Task<Result<PaginatedResult<GetTicketsQuery.Result>>> Handle(
        GetTicketsQuery query, CancellationToken cancellationToken)
    {
        Result<PersonId> currentPersonOrError = PrefixedGuidV3.From<PersonId>(query.CurrentPerson);
        if (currentPersonOrError.IsFailure)
        {
            return Result.Failure<PaginatedResult<GetTicketsQuery.Result>>(currentPersonOrError.Error);
        }

        Person? person = await peopleRepository.FindByIdAsync(currentPersonOrError.Value, cancellationToken);
        if (person is null)
        {
            return Result.Failure<PaginatedResult<GetTicketsQuery.Result>>(PeopleErrors.PersonNotFound);
        }

        PersonId? personIdFilter = NormalizedPersonIdFilter(query.FilterByPersonId, person);
        
        OrganId? organIdFilter = NormalizedOrganIdFilter(query.FilterByOrganId, person);
        
        bool onlyVisibleReplies = NormalizedRepliesVisibility(query.OnlyVisibleReplies, person);
        
        List<TicketView> ticketViews = await ticketingQueryService.GetAllAsync(
            query.SearchString,
            query.SortByPriority,
            onlyVisibleReplies,
            query.FilterByDepartment,
            query.FilterByStatus,
            personIdFilter,
            organIdFilter,
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

    private static PersonId? NormalizedPersonIdFilter(string? personIdFilter, Person person)
    {
        if (string.IsNullOrEmpty(personIdFilter))
        {
            return person.RoleType switch
            {
                RoleType.Admin => null,
                RoleType.Agent => null,
                RoleType.Client => person.Id,
                _ => person.Id
            };
        }
        Result<PersonId> personIdFilterOrError = PrefixedGuidV3.From<PersonId>(personIdFilter);
        if (personIdFilterOrError.IsFailure)
        {
            return person.RoleType switch
            {
                RoleType.Admin => null,
                RoleType.Agent => null,
                _ => person.Id
            };
        }
        
        return person.RoleType is RoleType.Admin or RoleType.Agent ? personIdFilterOrError.Value : person.Id;
        
    }
    private static OrganId? NormalizedOrganIdFilter(string? organIdFilter, Person person)
    {
        if (string.IsNullOrEmpty(organIdFilter))
        {
            return person.RoleType switch
            {
                RoleType.Admin => null,
                RoleType.Agent => person.OrganId,
                _ => null
            };
        }
        Result<OrganId> organIdFilterOrError = PrefixedGuidV3.From<OrganId>(organIdFilter);
        if (organIdFilterOrError.IsFailure)
        {
            return person.RoleType switch
            {
                RoleType.Admin => null,
                RoleType.Agent => person.OrganId,
                _ => null
            };
        }

        return person.RoleType switch
        {
            RoleType.Admin => organIdFilterOrError.Value,
            RoleType.Agent => person.OrganId,
            _ => null
        };
    }

    private static bool NormalizedRepliesVisibility(bool onlyVisibleReplies, Person person)
    {
        return person.RoleType switch
        {
            RoleType.Admin => onlyVisibleReplies,
            RoleType.Agent => false,
            _ => true
        };
    }
}
