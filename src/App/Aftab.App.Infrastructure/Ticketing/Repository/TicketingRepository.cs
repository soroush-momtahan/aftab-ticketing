using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.Ticketing.Repository;

internal sealed class TicketingRepository(
    AftabDbContext dbContext) : ITicketingRepository
{
    public async Task CreateAsync(Ticket ticket, CancellationToken cancellationToken)
    {
        await dbContext.Tickets.AddAsync(ticket, cancellationToken);
    }

    public async Task<Ticket?> GetByIdAsync(TicketId ticketId, CancellationToken cancellationToken)
    {
        return await dbContext.Tickets.SingleOrDefaultAsync(x => x.Id == ticketId, cancellationToken);
    }

    public async Task<ReplyTicket?> GetReplyTicketByIdAsync(ReplyTicketId replyTicketId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Tickets
            .AsNoTracking()
            .SelectMany(x => x.ReplyTickets)
            .SingleOrDefaultAsync(x => x.Id == replyTicketId, cancellationToken: cancellationToken);
    }
}
