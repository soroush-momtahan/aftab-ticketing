using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.ValueObjects;

namespace Aftab.App.Domain.Ticketing.Repository;

public interface ITicketingRepository
{
    Task CreateAsync(Ticket ticket, CancellationToken cancellationToken);
    Task<Ticket?> GetByIdAsync(TicketId ticketId, CancellationToken cancellationToken);
    Task<ReplyTicket?> GetReplyTicketByIdAsync(ReplyTicketId replyTicketId, CancellationToken cancellationToken);
}
