using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.App.Infrastructure.Data.ReadDb;
using Aftab.App.Infrastructure.Ticketing.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.Ticketing.Services;

internal sealed class TicketingQueryService(
    AftabReadDbContext dbContext) : ITicketingQueryService
{
    public async Task CreateAsync(TicketView ticketView, CancellationToken cancellationToken)
    {
        await dbContext.TicketViews.AddAsync(ticketView, cancellationToken);
    }

    public async Task<TicketView?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        return await dbContext.TicketViews.SingleOrDefaultAsync(x => x.Id == ticketId, cancellationToken);
    }

    public async Task<List<TicketView>> GetAllAsync(string? searchString, bool sortByPriority, TicketDepartment? filterByDepartment,
        TicketStatus? filterByStatus, PersonId? filterByPersonId, int pageIndex, int pageSize,
        CancellationToken cancellationToken)
    {
        List<TicketView> result = await dbContext.TicketViews
            .AsNoTracking()
            .ApplyFilters(searchString, filterByDepartment, filterByStatus, filterByPersonId)
            .ApplySorting(sortByPriority)
            .ApplyPagination(pageIndex, pageSize)
            .ToListAsync(cancellationToken);
        return result;
    }

    public void Update(TicketView ticketView)
    {
        dbContext.Update(ticketView);
    }

    public void Delete(TicketView ticketView)
    {
        dbContext.Remove(ticketView);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
