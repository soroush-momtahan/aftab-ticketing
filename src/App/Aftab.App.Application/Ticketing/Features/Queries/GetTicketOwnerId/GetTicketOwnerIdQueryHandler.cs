using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTicketOwnerId;

internal sealed class GetTicketOwnerIdQueryHandler(
    ITicketingRepository ticketingRepository) : IQueryHandler<GetTicketOwnerIdQuery, GetTicketOwnerIdQuery.Result>
{
    public async Task<Result<GetTicketOwnerIdQuery.Result>> Handle(
        GetTicketOwnerIdQuery query, CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(query.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure<GetTicketOwnerIdQuery.Result>(ticketIdOrError.Error);
        }
        
        Ticket? ticket = await ticketingRepository.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticket is null)
        {
            return Result.Failure<GetTicketOwnerIdQuery.Result>(TicketingErrors.NotFound(query.TicketId));
        }
        return new GetTicketOwnerIdQuery.Result(ticket.PersonId);
    }
}
