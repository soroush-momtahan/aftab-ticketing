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

namespace Aftab.App.Application.Ticketing.EventHandlers.ReplyTicketAdded;

internal sealed class UpdateTicketViewOnReplyTicketAddedEvent(
    ITicketingQueryService ticketingQueryService,
    ITicketingRepository ticketingRepository,
    IPeopleRepository peopleRepository) : DomainEventHandler<ReplyTicketAddedEvent>
{
    public override async Task Handle(
        ReplyTicketAddedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Ticket? ticket = await ticketingRepository.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketAddedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }
        
        TicketView? ticketView = await ticketingQueryService.GetByIdAsync(domainEvent.TicketId, cancellationToken);
        if (ticketView is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketAddedEvent),
                TicketingErrors.NotFound(domainEvent.TicketId));
        }

        ReplyTicket? replyTicket = ticket.ReplyTickets.SingleOrDefault(x => x.Id == domainEvent.ReplyTicketId);
        if (replyTicket is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketAddedEvent),
                TicketingErrors.ReplyTicketNotFound(domainEvent.ReplyTicketId));
        }

        Person? replierPerson = await peopleRepository.FindByIdAsync(replyTicket.ReplierId, cancellationToken);
        if (replierPerson is null)
        {
            throw new AftabException(
                nameof(UpdateTicketViewOnReplyTicketAddedEvent),
                PeopleErrors.PersonNotFound);
        }
        
        ticketView.Replies.Add(TicketView.ToReplyTicketView(replyTicket, replierPerson));
        ticketView.Status = ticket.Status;
        ticketingQueryService.Update(ticketView);
        await ticketingQueryService.SaveChangesAsync(cancellationToken);
    }
    
}
