using Aftab.Shared.Application.EventBus;
using Aftab.Shared.Infrastructure.Inbox.Models;
using Microsoft.EntityFrameworkCore;

namespace Aftab.Shared.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent, TDbContext>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    TDbContext dbContext) : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
    where TDbContext : DbContext
{
    public override async Task Handle(
        TIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken = default)
    {
        var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);
        if (await InboxConsumerExistsAsync(inboxMessageConsumer))
        {
            return;
        }
        await decorated.Handle(integrationEvent, cancellationToken);
        await InsertInboxConsumerAsync(inboxMessageConsumer);
    }
    private async Task<bool> InboxConsumerExistsAsync(
        InboxMessageConsumer inboxMessageConsumer)
    {
        bool result = await dbContext.Set<InboxMessageConsumer>()
            .AnyAsync(x=>
                x.Name == inboxMessageConsumer.Name &&
                x.InboxMessageId == inboxMessageConsumer.InboxMessageId);
        
        return result;
    }
    private async Task InsertInboxConsumerAsync(
        InboxMessageConsumer inboxMessageConsumer)
    {
        await dbContext.Set<InboxMessageConsumer>().AddAsync(inboxMessageConsumer);
        await dbContext.SaveChangesAsync();
    }
}
