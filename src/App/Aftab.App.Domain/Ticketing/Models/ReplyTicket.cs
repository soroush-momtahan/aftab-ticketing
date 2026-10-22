using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Domain.Objects;

namespace Aftab.App.Domain.Ticketing.Models;

public class ReplyTicket : Entity<ReplyTicketId>
{
    public TicketId TicketId { get; init; }
    public PersonId ReplierId { get; init; }
    public string Content { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool IsVisible { get; private set; }
    
    private ReplyTicket() {}

    private ReplyTicket(TicketId ticketId, PersonId replierId, string content)
    {
        TicketId = ticketId;
        ReplierId = replierId;
        Content = content;
    }

    internal static ReplyTicket Create(TicketId ticketId, PersonId replierId, string content)
    {
        return new ReplyTicket(
            ticketId,
            replierId,
            content);
    }

    internal void UpdateContent(string newContent)
    {
        Content = newContent;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void SoftDelete()
    {
        IsDeleted = true;
    }
    
    public void ChangeVisibility(bool isVisible)
    {
        IsVisible = isVisible;
    }
}
