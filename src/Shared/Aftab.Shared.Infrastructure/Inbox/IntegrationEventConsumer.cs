using Aftab.Shared.Application.EventBus;
using Aftab.Shared.Infrastructure.Inbox.Models;
using Aftab.Shared.Infrastructure.Serialization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Aftab.Shared.Infrastructure.Inbox;

public sealed class IntegrationEventConsumer<TIntegrationEvent, TDbContext>(TDbContext dbContext)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
    where TDbContext : DbContext
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        TIntegrationEvent integrationEvent = context.Message;

        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Instance),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        };
        await dbContext.Set<InboxMessage>().AddAsync(inboxMessage);
        await dbContext.SaveChangesAsync();
    }
}
