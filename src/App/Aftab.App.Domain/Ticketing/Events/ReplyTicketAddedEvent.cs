using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Domain.Events;

namespace Aftab.App.Domain.Ticketing.Events;

public class ReplyTicketAddedEvent(TicketId ticketId, ReplyTicketId replyTicketId) : DomainEvent
{
    public TicketId TicketId { get; } = ticketId;
    public ReplyTicketId ReplyTicketId { get; } = replyTicketId;
}
