using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Enums;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Events;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Domain.Objects;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Domain.Ticketing.Models;

public class Ticket : Aggregate<TicketId>
{
    public PersonId PersonId { get; init; }
    public OrganId? OrganId { get; private set; }
    public Title Title { get; init; }
    public string Content { get; private set; }
    public bool IsDeleted { get; private set; }
    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; init; }
    public TicketDepartment Department { get; init; }

    private readonly List<ReplyTicket> _replyTickets = [];
    public IReadOnlyList<ReplyTicket> ReplyTickets => _replyTickets.AsReadOnly();

    private Ticket() { }

    private Ticket(
        PersonId personId,
        Title title,
        string content,
        TicketPriority priority,
        TicketDepartment department)
    {
        PersonId = personId;
        Title = title;
        Content = content;
        Status = TicketStatus.Open;
        Priority = priority;
        Department = department;
    }

    public static Ticket Create(
        PersonId personId,
        Title title,
        string content,
        TicketPriority priority,
        TicketDepartment department)
    {
        var ticket = new Ticket(personId, title, content, priority, department);
        ticket.AddDomainEvent(new TicketCreatedEvent(ticket.Id));
        return ticket;
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new TicketContentUpdatedEvent(Id));
    }

    public void ChangeStatus(TicketStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new TicketStatusUpdatedEvent(Id));
    }

    public void AddReply(string content, PersonId replierId, TicketStatus status = TicketStatus.InProgress)
    {
        if (Status == TicketStatus.Closed)
        {
            return;   
        }
        var replyTicket = ReplyTicket.Create(Id, replierId, content);
        _replyTickets.Add(replyTicket);
        UpdatedAt = DateTime.UtcNow;
        bool isNotOwner = replierId != PersonId;
        if (isNotOwner)
        {
            Status = status;
        }

        AddDomainEvent(new ReplyTicketAddedEvent(Id, replyTicket.Id));
    }

    public Result UpdateReply(ReplyTicketId replyTicketId, string newContent)
    {
        ReplyTicket? replyTicket = _replyTickets.SingleOrDefault(x => x.Id == replyTicketId);
        if (replyTicket is null)
        {
            return Result.Failure(TicketingErrors.ReplyTicketNotFound(replyTicketId));
        }

        replyTicket.UpdateContent(newContent);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ReplyTicketContentUpdatedEvent(Id, replyTicketId));
        return Result.Success();
    }

    public Result SoftDeleteReply(ReplyTicketId replyTicketId)
    {
        ReplyTicket? replyTicket = _replyTickets.SingleOrDefault(x => x.Id == replyTicketId);
        if (replyTicket is null)
        {
            return Result.Failure(TicketingErrors.ReplyTicketNotFound(replyTicketId));
        }

        replyTicket.SoftDelete();
        AddDomainEvent(new ReplyTicketDeletedEvent(Id, replyTicketId));
        return Result.Success();
    }

    public void SoftDelete()
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow;
        AddDomainEvent(new TicketDeletedEvent(Id));
    }
    
    public void AssignOrgan(OrganId organId)
    {
        OrganId = organId;
        AddDomainEvent(new OrganAssignedToTicketEvent(Id, organId));
    }

    public Result ChangeReplyVisibility(
        ReplyTicketId replyTicketId,
        bool visibility)
    {
        ReplyTicket? replyTicket = _replyTickets.SingleOrDefault(x => x.Id == replyTicketId);
        if (replyTicket is null)
        {
            return Result.Failure(TicketingErrors.ReplyTicketNotFound(replyTicketId));
        }
        
        replyTicket.ChangeVisibility(visibility);
        AddDomainEvent(new ReplyTicketVisibilityChangedEvent(Id, replyTicketId));
        return Result.Success();
    }
}
