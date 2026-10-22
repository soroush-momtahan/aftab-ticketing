using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.OrganAssignedToTicket;

internal sealed class CreateTicketReporterViewOnOrganAssignedToTicket(
    ITicketingRepository ticketingRepository,
    ITicketingReporterQueryService ticketingReporterQueryService) : DomainEventHandler<OrganAssignedToTicketEvent>
{
    public override async Task Handle(
        OrganAssignedToTicketEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(CreateTicketReporterViewOnOrganAssignedToTicket),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        var ticketReporterView = new TicketReporterView()
        {
            OrganId = ticket.OrganId!,
        };
        ticketReporterView.Statuses.Add(TicketReporterView.ToTicketReporterStatus(ticket));
        
        await ticketingReporterQueryService.CreateAsync(ticketReporterView, cancellationToken);
        await ticketingReporterQueryService.SaveChangesAsync(cancellationToken);
    }
}
