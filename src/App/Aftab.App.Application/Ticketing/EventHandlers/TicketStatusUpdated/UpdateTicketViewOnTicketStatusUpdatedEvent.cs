using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.TicketStatusUpdated;

internal sealed class UpdateTicketViewOnTicketStatusUpdatedEvent(
    ITicketingQueryService ticketingQueryService,
    ITicketingRepository ticketingRepository)
    : DomainEventHandler<TicketStatusUpdatedEvent>
{
    public override async Task Handle(
        TicketStatusUpdatedEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnTicketStatusUpdatedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnTicketStatusUpdatedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        ticketView.Status = ticket.Status;
        ticketingQueryService.Update(ticketView);
        await ticketingQueryService.SaveChangesAsync(cancellationToken);
    }
}
