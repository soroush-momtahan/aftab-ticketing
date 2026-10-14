using Aftab.App.Domain.Ticketing.Enums;

namespace Aftab.App.Application.Ticketing.Services;

public class TicketView
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public TicketDepartment Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public List<ReplyTicket> Type { get; set; }
    public class ReplyTicket
    {
        public Guid Id { get; set; }
        public Guid ReplierId { get; set; }
        public string Content { get; set; }
    }
}
public interface ITicketingViewService
{
    Task CreateAsync(TicketView ticketView, CancellationToken cancellationToken);
    Task<TicketView?> GetById(Guid ticketId, CancellationToken cancellationToken);
}
