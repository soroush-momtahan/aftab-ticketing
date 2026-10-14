using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.ValueObjects;

namespace Aftab.App.Domain.Ticketing.Repository;

public interface ITicketRepository
{
    Task CreateAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdAsync(TicketId ticketId, CancellationToken cancellationToken);
    void SoftDelete(TicketId ticketId);
}
