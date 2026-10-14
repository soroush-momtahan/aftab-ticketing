namespace Aftab.Shared.Domain.Events;

public abstract class DomainEvent(Guid id, DateTime occuredOn) : IDomainEvent
{
    public Guid Id { get; init; } = id;
    public DateTime OccuredOn { get; init; } = occuredOn;
    protected DomainEvent() : this(Guid.CreateVersion7(), DateTime.UtcNow)
    {
    }
}
