using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.TicketContentUpdated;

internal sealed class UpdateTicketViewOnTicketContentUpdatedEvent(
    ITicketingViewService ticketingViewService,
    ITicketingRepository ticketingRepository)
    : DomainEventHandler<TicketContentUpdatedEvent>
{
    public override async Task Handle(
        TicketContentUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
         Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
         if (ticket is null)
         {
             throw new AftabException(
                 nameof(UpdateTicketViewOnTicketContentUpdatedEvent), 
                 TicketingErrors.NotFound(domainEvent.TicketId));
         }
         
         TicketView? ticketView = await ticketingViewService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
         if (ticketView is null)
         {
             throw new AftabException(
                 nameof(UpdateTicketViewOnTicketContentUpdatedEvent), 
                 TicketingErrors.NotFound(domainEvent.TicketId));
         }

         ticketView.Content = ticket.Content;
         ticketingViewService.Update(ticketView);
         await ticketingViewService.SaveChangesAsync(cancellationToken);
    }
}
