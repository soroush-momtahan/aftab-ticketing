using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Organization.Errors;
using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.Repository;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.OrganAssignedToTicket;

internal sealed class CreateTicketReporterViewOnOrganAssignedToTicket(
    ITicketingRepository ticketingRepository,
    IOrganizationRepository organizationRepository,
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

        Organ? organ = await organizationRepository.GetByIdAsync(domainEvent.OrganId, cancellationToken);
        if (organ is null)
        {
            throw new AftabException(
                nameof(CreateTicketReporterViewOnOrganAssignedToTicket),
                OrganizationErrors.NotFound);
        }
        
        var ticketReporterView = TicketReporterView.ToTicketReporterView(ticket, organ);
        
        await ticketingReporterQueryService.CreateAsync(ticketReporterView, cancellationToken);
        await ticketingReporterQueryService.SaveChangesAsync(cancellationToken);
    }
}
