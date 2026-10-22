using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.Organization.ValueObjects;
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

    public async Task<List<TicketView>> GetAllAsync(
        string? searchString,
        bool sortByPriority,
        bool onlyVisibleReplies,
        TicketDepartment? filterByDepartment,
        TicketStatus? filterByStatus,
        PersonId? filterByPersonId,
        OrganId? filterByOrganId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken)
    {
        List<TicketView> result = await dbContext.TicketViews
            .AsNoTracking()
            .ApplyFilters(
                searchString,
                filterByDepartment,
                filterByStatus,
                filterByPersonId,
                filterByOrganId)
            .ApplySorting(sortByPriority)
            .ApplyPagination(pageIndex, pageSize)
            .Select(x => new TicketView()
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                DeletedAt = x.DeletedAt,
                OrganId = x.OrganId,
                PersonId = x.PersonId,
                Department = x.Department,
                DisplayName = x.DisplayName,
                ModifiedAt = x.ModifiedAt,
                Priority = x.Priority,
                Status = x.Status,
                Title = x.Title,
                Replies = onlyVisibleReplies ? 
                    x.Replies.Where(r => r.IsVisible == onlyVisibleReplies || r.ReplierId == x.PersonId).ToList() :
                    x.Replies.ToList()
            })
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
