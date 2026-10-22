using Aftab.App.Domain.Ticketing.Enums;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.ValueObjects;

namespace Aftab.App.Application.Ticketing.Services;

public class TicketReporterView
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string? OrganId { get; set; }
    public List<TicketReporterStatusView> Statuses { get; set; } = [];
    public int TotalStatusCount => Statuses.Count;
    public int OpenTickets => Statuses.Count(x => x.Status == TicketStatus.Open);
    public int InprogressTickets => Statuses.Count(x => x.Status == TicketStatus.InProgress);
    public int AnsweredTickets => Statuses.Count(x => x.Status == TicketStatus.Answered);
    public int ClosedTickets => Statuses.Count(x => x.Status == TicketStatus.Closed);
    
    public static TicketReporterView ToTicketReporterView(Ticket ticket)
    {
        
        return new TicketReporterView()
        {
            OrganId = ticket.OrganId ?? string.Empty,
            Statuses = []
        };
    }

    public static TicketReporterStatusView ToTicketReporterStatus(Ticket ticket)
    {
        return new TicketReporterStatusView()
        {
            TicketId = ticket.Id,
            Status = ticket.Status
        };
    }
}

public class TicketReporterStatusView
{
    public string TicketId { get; set; }
    public TicketStatus Status { get; set; }
    
}

public interface ITicketingReporterQueryService
{
    Task CreateAsync(TicketReporterView ticketReporterView, CancellationToken cancellationToken);
    Task<TicketReporterView?> GetByTicketIdAsync(TicketId ticketId, CancellationToken cancellationToken);
    void Update(TicketReporterView ticketReporterView);
    void Delete(TicketReporterView ticketReporterView);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
