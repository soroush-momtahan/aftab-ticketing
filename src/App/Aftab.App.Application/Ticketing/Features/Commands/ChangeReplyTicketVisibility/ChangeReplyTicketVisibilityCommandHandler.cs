using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Commands.ChangeReplyTicketVisibility;

internal sealed class ChangeReplyTicketVisibilityCommandHandler(
    ITicketingRepository ticketingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangeReplyTicketVisibilityCommand>
{
    public async Task<Result> Handle(
        ChangeReplyTicketVisibilityCommand command, 
        CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(command.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            Result.Failure(ticketIdOrError.Error);
        }
        
        Result<ReplyTicketId> replyTicketIdOrError = PrefixedGuidV3.From<ReplyTicketId>(command.ReplyTicketId);
        if (replyTicketIdOrError.IsFailure)
        {
            Result.Failure(replyTicketIdOrError.Error);
        }

        Ticket? ticket = await ticketingRepository.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticket is null)
        {
            return Result.Failure(TicketingErrors.NotFound(command.TicketId));
        }
        
        ticket.ChangeReplyVisibility(
            replyTicketIdOrError.Value,
            command.Visibility);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
