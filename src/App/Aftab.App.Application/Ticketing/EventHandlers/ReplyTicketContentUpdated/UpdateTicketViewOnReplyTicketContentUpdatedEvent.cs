using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.ReplyTicketContentUpdated;

internal sealed class UpdateTicketViewOnReplyTicketContentUpdatedEvent(
    ITicketingViewService ticketingViewService,
    ITicketingRepository ticketingRepository)
    : DomainEventHandler<ReplyTicketContentUpdatedEvent>
{
    public override async Task Handle(
        ReplyTicketContentUpdatedEvent domainEvent, 
        CancellationToken cancellationToken = default)
    { 
        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketContentUpdatedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        ReplyTicket? replyTicket = ticket.ReplyTickets.SingleOrDefault(x => x.ReplierId == domainEvent.ReplyTicketId);
        if (replyTicket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketContentUpdatedEvent),
                TicketingErrors.ReplyTicketNotFound(domainEvent.ReplyTicketId));
        }

        TicketView? ticketView = await ticketingViewService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketContentUpdatedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        TicketView.ReplyTicketView? replyTicketView = ticketView.Replies.SingleOrDefault(
            x => x.ReplierId == domainEvent.ReplyTicketId);
        if (replyTicketView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketContentUpdatedEvent),
                TicketingErrors.ReplyTicketNotFound(domainEvent.ReplyTicketId));
        }
        
        replyTicketView.Content = replyTicket.Content;
        ticketingViewService.Update(ticketView);
        await ticketingViewService.SaveChangesAsync(cancellationToken);
    }
}
