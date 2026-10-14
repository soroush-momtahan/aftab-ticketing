namespace Aftab.Shared.Application.Messaging;

public interface IApplicationEventHandler<in TApplicationEvent>
    : IApplicationEventHandler
    where TApplicationEvent : IApplicationEvent
{
    Task Handle(TApplicationEvent applicationEvent, CancellationToken cancellationToken);
}

public interface IApplicationEventHandler
{
    Task Handle(IApplicationEvent applicationEvent, CancellationToken cancellationToken);
}
