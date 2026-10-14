namespace Aftab.Shared.Application.Messaging;

public abstract class ApplicationEventHandler<TApplicationEvent> : IApplicationEventHandler<TApplicationEvent>
where TApplicationEvent : ApplicationEvent
{

    public abstract Task Handle(
        TApplicationEvent applicationEvent,
        CancellationToken cancellationToken);
    
    public Task Handle(IApplicationEvent applicationEvent, CancellationToken cancellationToken)
    {
        return Handle((TApplicationEvent)applicationEvent, cancellationToken);
    }
}
