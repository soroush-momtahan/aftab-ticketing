using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.TicketStatusUpdated;

internal sealed class UpdateTicketReporterViewOnTicketStatusUpdate(
    ITicketingReporterQueryService ticketingReporterQueryService,
    ITicketingRepository ticketingRepository) : DomainEventHandler<TicketStatusUpdatedEvent>
{
    public override async Task Handle(TicketStatusUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketReporterViewOnTicketStatusUpdate),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        TicketReporterView? ticketReporterView = await ticketingReporterQueryService.GetByTicketIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketReporterView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketReporterViewOnTicketStatusUpdate),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        TicketReporterStatusView? ticketReporterStatusView =
            ticketReporterView.Statuses.SingleOrDefault(x => x.TicketId == domainEvent.TicketId);
        if (ticketReporterStatusView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketReporterViewOnTicketStatusUpdate),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        ticketReporterStatusView.Status = ticket.Status;
        
        ticketingReporterQueryService.Update(ticketReporterView);
        await ticketingReporterQueryService.SaveChangesAsync(cancellationToken);
    }
}
