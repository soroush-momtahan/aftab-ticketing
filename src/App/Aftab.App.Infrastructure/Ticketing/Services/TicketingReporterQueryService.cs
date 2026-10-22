using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.App.Infrastructure.Data.ReadDb;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.Ticketing.Services;

public class TicketingReporterQueryService(
    AftabReadDbContext dbContext) : ITicketingReporterQueryService
{
    public async Task CreateAsync(TicketReporterView ticketReporterView, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(ticketReporterView, cancellationToken);
    }

    public async Task<TicketReporterView?> GetByTicketIdAsync(TicketId ticketId, CancellationToken cancellationToken)
    {

        return await dbContext.TicketReporterViews.FirstOrDefaultAsync(t => EF.Functions.JsonContains(
                t.Statuses,
                new TicketReporterStatusView[] { new() { TicketId = ticketId } }), cancellationToken);
    }

    public async Task<List<TicketReporterView>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.TicketReporterViews.ToListAsync(cancellationToken);
    }

    public void Update(TicketReporterView ticketReporterView)
    {
        dbContext.Update(ticketReporterView);
    }

    public void Delete(TicketReporterView ticketReporterView)
    {
        dbContext.Remove(ticketReporterView);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
