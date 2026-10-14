using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Exceptions;

namespace Aftab.App.Application.Ticketing.EventHandlers.TicketCreated;

internal sealed class CreateTicketViewOnTicketCreatedEvent(
    ITicketingViewService ticketingViewService,
    ITicketingRepository ticketingRepository,
    IPeopleRepository peopleRepository) : DomainEventHandler<TicketCreatedEvent>
{
    public override async Task Handle(
        TicketCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(CreateTicketViewOnTicketCreatedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        Person? person = await peopleRepository.FindByIdAsync(ticket.PersonId, cancellationToken);
        if (person is null)
        {
            throw new AftabException(
                nameof(CreateTicketViewOnTicketCreatedEvent),
                PeopleErrors.PersonNotFound);
        }
        
        await ticketingViewService.CreateAsync(TicketView.ToTicketView(ticket, person), cancellationToken);
        await ticketingViewService.SaveChangesAsync(cancellationToken);
    }
}
