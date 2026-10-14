using Aftab.Shared.Application.EventBus;
using MassTransit;

namespace Aftab.Shared.Infrastructure.EventBus;

internal sealed class EventBus(IBus bus) : IEventBus
{

    public async Task PublishAsync<T>(
        T integrationEvent, 
        CancellationToken cancellationToken = default) 
        where T : IIntegrationEvent
    {
        await bus.Publish(integrationEvent, cancellationToken);
    }
}
