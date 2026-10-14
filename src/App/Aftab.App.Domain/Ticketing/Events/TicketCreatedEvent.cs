using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Domain.Events;

namespace Aftab.App.Domain.Ticketing.Events;

public class TicketCreatedEvent(TicketId ticketId) : DomainEvent
{
    public TicketId TicketId { get; } = ticketId;
}
