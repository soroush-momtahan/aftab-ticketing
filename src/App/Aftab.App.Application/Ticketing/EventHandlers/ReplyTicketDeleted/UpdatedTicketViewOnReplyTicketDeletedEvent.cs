using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.ReplyTicketDeleted;

internal sealed class UpdatedTicketViewOnReplyTicketDeletedEvent(
    ITicketingQueryService ticketingQueryService)
    : DomainEventHandler<ReplyTicketDeletedEvent>
{
    public override async Task Handle(
        ReplyTicketDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {
            throw new AftabException(
                nameof(UpdatedTicketViewOnReplyTicketDeletedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        TicketView.ReplyTicketView? replyTicketView = ticketView.Replies.SingleOrDefault(
            x => x.Id == domainEvent.ReplyTicketId);
        if (replyTicketView is null)
        {
            throw new AftabException(
                nameof(UpdatedTicketViewOnReplyTicketDeletedEvent),
                TicketingErrors.NotFound(domainEvent.ReplyTicketId));
        }

        ticketView.Replies.Remove(replyTicketView);
        ticketingQueryService.Update(ticketView);
        await ticketingQueryService.SaveChangesAsync(cancellationToken);
    }
}
