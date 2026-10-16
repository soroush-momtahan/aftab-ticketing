using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.TicketDeleted;

internal sealed class DeleteTicketViewOnTicketDeletedEvent(
    ITicketingQueryService ticketingQueryService)
    : DomainEventHandler<TicketDeletedEvent>
{
    public override async Task Handle(
        TicketDeletedEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {
            throw new AftabException(
                nameof(DeleteTicketViewOnTicketDeletedEvent), 
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        ticketingQueryService.Delete(ticketView);
        await ticketingQueryService.SaveChangesAsync(cancellationToken);
    }
}
