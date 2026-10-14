using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Commands.SoftDeleteReplyTicket;

internal sealed class SoftDeleteReplyTicketCommandHandler(
    ITicketingRepository ticketingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SoftDeleteReplyTicketCommand>
{
    public async Task<Result> Handle(
        SoftDeleteReplyTicketCommand command, CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(command.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure(ticketIdOrError.Error);
        }

        Result<ReplyTicketId> replyIdOrError = PrefixedGuidV3.From<ReplyTicketId>(command.ReplyTicketId);
        if (replyIdOrError.IsFailure)
        {
            return Result.Failure(replyIdOrError.Error);
        }

        Ticket? ticket = await ticketingRepository.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticket is null)
        {
            return Result.Failure(TicketingErrors.NotFound(command.TicketId));
        }

        Result deleteReplyOrError = ticket.SoftDeleteReply(replyIdOrError.Value);
        if (deleteReplyOrError.IsFailure)
        {
            return Result.Failure(deleteReplyOrError.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
