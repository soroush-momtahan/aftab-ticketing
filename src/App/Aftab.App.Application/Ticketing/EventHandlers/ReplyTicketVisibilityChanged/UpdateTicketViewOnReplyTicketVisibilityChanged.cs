using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.ReplyTicketVisibilityChanged;

internal sealed class UpdateTicketViewOnReplyTicketVisibilityChanged(
    ITicketingQueryService ticketingQueryService,
    ITicketingRepository ticketingRepository) : DomainEventHandler<ReplyTicketVisibilityChangedEvent>
{
    public override async Task Handle(
        ReplyTicketVisibilityChangedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {  
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketVisibilityChanged),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketVisibilityChanged),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        
        ReplyTicket? replyTicket = ticket.ReplyTickets.SingleOrDefault(x => x.Id == domainEvent.ReplyTicketId);
        if (replyTicket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketVisibilityChanged),
                TicketingErrors.ReplyTicketNotFound(domainEvent.ReplyTicketId));
        }
        
        TicketView.ReplyTicketView? replyTicketView = ticketView.Replies.SingleOrDefault(
            x => x.Id == domainEvent.ReplyTicketId);
        if (replyTicketView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketVisibilityChanged),
                TicketingErrors.NotFound(domainEvent.ReplyTicketId));
        }

        replyTicketView.IsVisible = replyTicket.IsVisible;
        ticketingQueryService.Update(ticketView);
        await ticketingQueryService.SaveChangesAsync(cancellationToken);
    }
}
