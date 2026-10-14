namespace Aftab.Shared.Application.Messaging;

public abstract class ApplicationEvent(Guid id, DateTime occuredOn) : IApplicationEvent
{
    public Guid Id { get; } = id;
    public DateTime OccuredOn { get; } = occuredOn;
    protected ApplicationEvent() : this(Guid.NewGuid(), DateTime.Now){}
}
