using Aftab.Shared.Application.EventBus;
using Aftab.Shared.Infrastructure.Inbox.Models;
using Aftab.Shared.Infrastructure.Inbox.OptionsSetups;
using Aftab.Shared.Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Aftab.Shared.Infrastructure.Inbox;

[DisallowConcurrentExecution]
public sealed class ProcessInboxJob<TDbContext>(
    TDbContext dbContext,
    IOptions<InboxOptions> inboxOptions,
    InboxModuleConfig moduleConfig,
    IServiceScopeFactory scopeFactory,
    ILogger<ProcessInboxJob<TDbContext>> logger) : IJob where TDbContext : DbContext
{
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;
    private readonly int _totalSteps = 4;
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process inbox messages", moduleConfig.ModuleName);
        
        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async (ct) =>
        {
            await using IDbContextTransaction transaction = await
                dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                logger.LogInformation("Step 1/{TotalSteps}: Retrieving inbox messages to process ...", _totalSteps);
                IReadOnlyList<InboxMessage> inboxMessages =
                    await GetInboxMessagesAsync(ct);
                
                if (!inboxMessages.Any())
                {
                    logger.LogInformation("{Module} - inbox was empty and process completed with no actions", moduleConfig.ModuleName);
                    return;
                }
                
                logger.LogInformation(
                    "Step 2/{TotalSteps}: Processing {Count} inbox messages ...",
                    _totalSteps,
                    inboxMessages.Count);

                foreach (InboxMessage inboxMessage in inboxMessages)
                {
                    Exception? exception = null;
                    try
                    {
                        IIntegrationEvent integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(  
                            inboxMessage.Content,  
                            SerializerSettings.Instance)!; 
                        
                        using IServiceScope scope = scopeFactory.CreateScope();
                        
                        IEnumerable<IIntegrationEventHandler> handlers = IntegrationEventHandlersFactory.GetHandlers(  
                            integrationEvent.GetType(),  
                            scope.ServiceProvider);
                        
                        IEnumerable<IIntegrationEventHandler> integrationEventHandlers = handlers.ToList();

                        foreach (IIntegrationEventHandler integrationEventHandler in integrationEventHandlers)
                        {
                            await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);
                        }
                    }
                    catch (Exception caughtException)
                    {
                        logger.LogError(
                            caughtException,
                            "{Module} - Exception while processing inbox message {MessageId} ...",
                            moduleConfig.ModuleName,
                            inboxMessage.Id);

                        exception = caughtException;
                    }
                    logger.LogInformation(
                        "Step 3/{TotalSteps}: updating inbox message {MessageId} of type {MessageType} as processed ...",
                        _totalSteps,
                        inboxMessage.Id,
                        inboxMessage.Type);
                    
                    await UpdateInboxMessageAsync(inboxMessage, exception, ct);
                }
                
                logger.LogInformation("Step 4/{TotalSteps}: Committing transaction ...", _totalSteps);
                await transaction.CommitAsync(ct);
                logger.LogInformation("{Module} - Completed processing inbox messages", moduleConfig.ModuleName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, context.CancellationToken);
    }
    private async Task<IReadOnlyList<InboxMessage>> GetInboxMessagesAsync(
        CancellationToken cancellationToken = default)
    {
        List<InboxMessage> inboxMessages = await dbContext.Set<InboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.ProcessedOnUtc)
            .Take(_inboxOptions.BatchSize)
            .ToListAsync(cancellationToken);
        return [.. inboxMessages];
    }
    private async Task UpdateInboxMessageAsync(
        InboxMessage inboxMessage,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        inboxMessage.AddProcessedOn(DateTime.UtcNow);
        inboxMessage.AddError(exception?.ToString());
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
