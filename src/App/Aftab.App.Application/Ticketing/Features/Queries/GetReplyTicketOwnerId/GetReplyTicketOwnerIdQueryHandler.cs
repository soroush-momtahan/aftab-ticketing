using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetReplyTicketOwnerId;

internal sealed class GetReplyTicketOwnerIdQueryHandler(
    ITicketingRepository ticketingRepository) : IQueryHandler<GetReplyTicketOwnerIdQuery, GetReplyTicketOwnerIdQuery.Result>
{
    public async Task<Result<GetReplyTicketOwnerIdQuery.Result>> Handle(
        GetReplyTicketOwnerIdQuery query, CancellationToken cancellationToken)
    {
        Result<ReplyTicketId> replyTicketIdOrError = PrefixedGuidV3.From<ReplyTicketId>(query.ReplyTicketId);
        if (replyTicketIdOrError.IsFailure)
        {
            return Result.Failure<GetReplyTicketOwnerIdQuery.Result>(replyTicketIdOrError.Error);
        }
        
        ReplyTicket? replyTicket = await ticketingRepository.GetReplyTicketByIdAsync(
            replyTicketIdOrError.Value, cancellationToken);
        if (replyTicket is null)
        {
            return Result.Failure<GetReplyTicketOwnerIdQuery.Result>(
                TicketingErrors.ReplyTicketNotFound(query.ReplyTicketId));
        }

        return new GetReplyTicketOwnerIdQuery.Result(replyTicket.ReplierId);
    }
}
