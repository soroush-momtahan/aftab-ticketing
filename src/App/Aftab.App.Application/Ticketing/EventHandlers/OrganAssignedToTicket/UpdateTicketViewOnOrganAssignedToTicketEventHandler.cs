using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.OrganAssignedToTicket;

internal sealed class UpdateTicketViewOnOrganAssignedToTicketEventHandler(
    ITicketingQueryService ticketingQueryService,
    ITicketingRepository ticketingRepository) : DomainEventHandler<OrganAssignedToTicketEvent>
{
    public override async Task Handle(
        OrganAssignedToTicketEvent domainEvent, CancellationToken cancellationToken = default)
    {
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnOrganAssignedToTicketEventHandler),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnOrganAssignedToTicketEventHandler),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        ticketView.OrganId = ticket.OrganId;
        
        ticketingQueryService.Update(ticketView);
        await ticketingQueryService.SaveChangesAsync(cancellationToken);
    }
}
