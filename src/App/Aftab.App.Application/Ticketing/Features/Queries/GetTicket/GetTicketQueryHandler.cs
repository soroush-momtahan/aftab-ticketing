using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetTicket;

internal sealed class GetTicketQueryHandler(
    ITicketingQueryService ticketingQueryService) : ICommandHandler<GetTicketQuery, GetTicketQuery.Result>
{
    public async Task<Result<GetTicketQuery.Result>> Handle(
        GetTicketQuery request, CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(request.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure<GetTicketQuery.Result>(ticketIdOrError.Error);
        }
        
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticketView is null)
        {
            return Result.Failure<GetTicketQuery.Result>(TicketingErrors.NotFound(request.TicketId));
        }

        return GetTicketQuery.ToResult(ticketView);
    }
}
