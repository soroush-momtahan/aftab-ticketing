using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.App.Infrastructure.Data.ReadDb;

namespace Aftab.App.Infrastructure.Ticketing.Services;

public class TicketingReporterQueryService(
    AftabReadDbContext dbContext) : ITicketingReporterQueryService
{
    public async Task CreateAsync(TicketReporterView ticketReporterView, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(ticketReporterView, cancellationToken);
    }

    public Task<TicketReporterView?> GetByTicketIdAsync(TicketId ticketId, CancellationToken cancellationToken)
    {
        
        throw new NotImplementedException();
    }

    public void Update(TicketReporterView ticketReporterView)
    {
        throw new NotImplementedException();
    }

    public void Delete(TicketReporterView ticketReporterView)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
