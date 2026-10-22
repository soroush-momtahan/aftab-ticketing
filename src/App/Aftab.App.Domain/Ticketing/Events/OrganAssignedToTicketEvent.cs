using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Domain.Events;

namespace Aftab.App.Domain.Ticketing.Events;

public class OrganAssignedToTicketEvent(TicketId ticketId, OrganId organId) : DomainEvent
{
    public TicketId TicketId { get; } = ticketId;
    public OrganId OrganId { get; } = organId;
}
