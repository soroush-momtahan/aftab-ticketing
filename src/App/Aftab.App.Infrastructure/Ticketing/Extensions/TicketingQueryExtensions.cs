using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Enums;

namespace Aftab.App.Infrastructure.Ticketing.Extensions;

public static class TicketingQueryExtensions
{
     public static IQueryable<TicketView> ApplyFilters(
        this IQueryable<TicketView> query,
        string? searchString,
        TicketDepartment? department,
        TicketStatus? status,
        PersonId? personId)
    {
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(t => 
                t.Title.Contains(searchString) || 
                t.Content.Contains(searchString));
        }

        // فیلتر دپارتمان
        if (department.HasValue)
        {
            query = query.Where(t => t.Department == department.Value);
        }

        // فیلتر وضعیت
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        // فیلتر شخص (نکته: باید PersonId/OwnerId در TicketView وجود داشته باشد)
        // اگر منظور جستجو در Replyهاست: query.Where(t <= t.Replies.Any(r <= r.ReplierId.Value.ToString() == personId))
        
        if (personId != null)
        {
            query = query.Where(t => t.PersonId == personId);
        }

        return query;
    }

    public static IQueryable<TicketView> ApplySorting(
        this IQueryable<TicketView> query, 
        bool sortByPriority)
    {
        if (sortByPriority)
        {
            return query.OrderByDescending(t => t.Priority)
                        .ThenByDescending(t => t.CreatedAt); 
        }

        return query.OrderByDescending(t => t.CreatedAt);
    }

    public static IQueryable<TicketView> ApplyPagination(
        this IQueryable<TicketView> query, 
        int pageIndex, 
        int pageSize)
    {
        int skip = (pageIndex - 1) * pageSize;
        
        return query.Skip(skip > 0 ? skip : 0).Take(pageSize);
    }
}
