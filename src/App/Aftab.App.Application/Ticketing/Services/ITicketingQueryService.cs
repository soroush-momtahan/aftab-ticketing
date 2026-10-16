using Aftab.App.Domain.Identity.Enums;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.ValueObjects;

namespace Aftab.App.Application.Ticketing.Services;

public class TicketView
{
    public TicketView() { }
    public TicketId Id { get; set; }
    public PersonId PersonId { get; set; }
    public string DisplayName { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public TicketDepartment Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public List<ReplyTicketView> Replies { get; set; }
    public class ReplyTicketView
    {
        public ReplyTicketView() { }
        public ReplyTicketId Id { get; set; }
        public PersonId ReplierId { get; set; }
        public string ReplierDisplayName { get; set; }
        public string Content { get; set; }
    }
    public static TicketView ToTicketView(Ticket ticket, Person person)
    {
        string ticketerRole = person.RoleType == RoleType.Agent ? "مدیر" : "کاربر";
        return new TicketView()
        {
            Id = ticket.Id,
            PersonId = ticket.PersonId,
            DisplayName = $"{person.FullName.Firstname} {person.FullName.Lastname}({ticketerRole})",
            Title = ticket.Title.Value,
            Content = ticket.Content,
            Priority = ticket.Priority,
            Status = ticket.Status,
            Department = ticket.Department,
            CreatedAt = person.CreatedAt,
            Replies = []
        };
    }
    public static ReplyTicketView ToReplyTicketView(ReplyTicket replyTicket, Person person)
    {
        string ticketerRole = person.RoleType == RoleType.Agent ? "مدیر" : "کاربر";
        return new TicketView.ReplyTicketView()
        {
            Id = replyTicket.Id,
            ReplierId = replyTicket.ReplierId,
            ReplierDisplayName = $"{person.FullName.Firstname} {person.FullName.Lastname}({ticketerRole})",
            Content = replyTicket.Content,
        };
    }
    
}
public interface ITicketingQueryService
{
    Task CreateAsync(TicketView ticketView, CancellationToken cancellationToken);
    Task<TicketView?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken);
    Task<List<TicketView>> GetAllAsync(
        string? searchString,
        bool sortByPriority,
        TicketDepartment? filterByDepartment,
        TicketStatus? filterByStatus,
        PersonId? filterByPersonId,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken);
    void Update(TicketView ticketView);
    void Delete(TicketView ticketView);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
