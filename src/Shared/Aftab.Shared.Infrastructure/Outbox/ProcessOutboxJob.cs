using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Domain.Events;
using Aftab.Shared.Infrastructure.Outbox.Models;
using Aftab.Shared.Infrastructure.Outbox.OptionsSetups;
using Aftab.Shared.Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Aftab.Shared.Infrastructure.Outbox;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxJob<TDbContext>(
    TDbContext dbContext,
    IOptions<OutboxOptions> outboxOptions,
    OutboxModuleConfig moduleConfig,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<ProcessOutboxJob<TDbContext>> logger) : IJob where TDbContext : DbContext
{
    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;
    private readonly int _totalSteps = 4;
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process outbox messages", moduleConfig.ModuleName);

        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
        
        await strategy.ExecuteAsync(async (ct) =>
        {
            // ❌ خطوط مربوط به BeginTransaction حذف شد
            
            try
            {
                logger.LogInformation("Step 1/{TotalSteps}: Retrieving outbox messages to process ...", _totalSteps);
                IReadOnlyList<OutboxMessage> outboxMessages =
                    await GetOutboxMessagesAsync(ct);
                
                if (!outboxMessages.Any())
                {
                    logger.LogInformation("{Module} - outbox was empty and process completed with no actions", moduleConfig.ModuleName);
                    return;
                }
                
                logger.LogInformation(
                    "Step 2/{TotalSteps}: Processing {Count} outbox messages ...",
                    _totalSteps,
                    outboxMessages.Count);
                
                foreach (OutboxMessage outboxMessage in outboxMessages)
                {
                    Exception? exception = null;
                    try
                    {
                        // ... (کدهای Deserialize و پیدا کردن Handler ها دقیقاً مثل قبل باقی می‌ماند) ...
                        
                        IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                            outboxMessage.Content,
                            SerializerSettings.Instance)!;

                        using IServiceScope scope = serviceScopeFactory.CreateScope();

                        IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
                            domainEvent.GetType(),
                            scope.ServiceProvider);

                        IEnumerable<IDomainEventHandler> domainEventHandlers = handlers.ToList();
                        
                        foreach (IDomainEventHandler domainEventHandler in domainEventHandlers)
                        {
                            // هر هندلر دیتای خودش و وضعیت Idempotency خودش را مستقل SaveChanges میکند
                            await domainEventHandler.Handle(domainEvent, context.CancellationToken);
                        }
                    }
                    catch (Exception caughtException)
                    {
                        logger.LogError(
                            caughtException,
                            "{Module} - Exception while processing outbox message {MessageId} ...",
                            moduleConfig.ModuleName,
                            outboxMessage.Id);

                        exception = caughtException;
                    }
                    
                    logger.LogInformation(
                        "Step 3/{TotalSteps}: updating outbox message {MessageId} of type {MessageType} as processed ...",
                        _totalSteps,
                        outboxMessage.Id,
                        outboxMessage.Type);
                    
                    // این متد وضعیت OutboxMessage را در dbContext مربوط به Job ذخیره میکند
                    await UpdateOutboxMessageAsync(outboxMessage, exception, ct);
                }

                // ❌ خط مربوط به Commit کردن Transaction حذف شد
                logger.LogInformation("{Module} - Completed processing outbox messages", moduleConfig.ModuleName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, context.CancellationToken);
    }

    private async Task<IReadOnlyList<OutboxMessage>> GetOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        List<OutboxMessage> outboxMessages = await dbContext.Set<OutboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.ProcessedOnUtc)
            .Take(_outboxOptions.BatchSize)
            .ToListAsync(cancellationToken);
        return [.. outboxMessages];
    }

    private async Task UpdateOutboxMessageAsync(
        OutboxMessage outboxMessage,
        Exception? exception,
        CancellationToken cancellationToken)
    {
        if (exception == null)
        {
            // فقط اگر همه هندلرها موفق بودند، پیام را به عنوان پردازش شده علامت بزن
            outboxMessage.AddProcessedOn(DateTime.UtcNow);
        }
        else
        {
            // اگر خطا داشت، فقط ارور را ثبت کن تا جاب بعدی Quartz دوباره آن را بخواند و Retry کند
            outboxMessage.AddError(exception.ToString());
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
// Execute With Transaction
#pragma warning disable S125
// public async Task Execute(IJobExecutionContext context)
//     {
//         logger.LogInformation("{Module} - Beginning to process outbox messages", moduleConfig.ModuleName);
//
//         IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
//         
//         await strategy.ExecuteAsync(async (ct) =>
//         {
//             await using IDbContextTransaction transaction = await
//                 dbContext.Database.BeginTransactionAsync(ct);
//             
//             try
//             {
//                 logger.LogInformation("Step 1/{TotalSteps}: Retrieving outbox messages to process ...", _totalSteps);
//                 IReadOnlyList<OutboxMessage> outboxMessages =
//                     await GetOutboxMessagesAsync(ct);
//                 
//                 if (!outboxMessages.Any())
//                 {
//                     logger.LogInformation("{Module} - outbox was empty and process completed with no actions", moduleConfig.ModuleName);
//                     return;
//                 }
//                 
//                 logger.LogInformation(
//                     "Step 2/{TotalSteps}: Processing {Count} outbox messages ...",
//                     _totalSteps,
//                     outboxMessages.Count);
//                 
//                 foreach (OutboxMessage outboxMessage in outboxMessages)
//                 {
//                     Exception? exception = null;
//                     try
//                     {
//                         logger.LogInformation(
//                             "Step 2/{TotalSteps}: desrializing outbox message {MessageId} of type {MessageType} ...",
//                             _totalSteps,
//                             outboxMessage.Id,
//                             outboxMessage.Type);
//                         
//                         IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
//                             outboxMessage.Content,
//                             SerializerSettings.Instance)!;
//
//                         using IServiceScope scope = serviceScopeFactory.CreateScope();
//
//                         logger.LogInformation(
//                             "Step 2/{TotalSteps}: retrieving handlers for outbox message {MessageId} of type {MessageType} ...",
//                             _totalSteps,
//                             outboxMessage.Id,
//                             outboxMessage.Type);
//                         IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
//                             domainEvent.GetType(),
//                             scope.ServiceProvider);
//
//                         IEnumerable<IDomainEventHandler> domainEventHandlers = handlers.ToList();
//                         
//                         logger.LogInformation(
//                             "Step 2/{TotalSteps}: processing outbox message {MessageId} of type {MessageType} with {HandlerCount} handlers ...",
//                             _totalSteps,
//                             outboxMessage.Type,
//                             domainEventHandlers.Count(),
//                             outboxMessage.Id);
//
//                         foreach (IDomainEventHandler domainEventHandler in domainEventHandlers)
//                         {
//                             logger.LogInformation(
//                                 "Step 2/{TotalSteps}: handling outbox message {MessageId} of type {MessageType} with handler {HandlerType} ...",
//                                 _totalSteps,
//                                 outboxMessage.Id,
//                                 outboxMessage.Type,
//                                 domainEventHandler.GetType().FullName);
//                             
//                             await domainEventHandler.Handle(domainEvent, context.CancellationToken);
//                         }
//                     }
//                     catch (Exception caughtException)
//                     {
//                         logger.LogError(
//                             caughtException,
//                             "{Module} - Exception while processing outbox message {MessageId} ...",
//                             moduleConfig.ModuleName,
//                             outboxMessage.Id);
//
//                         exception = caughtException;
//                     }
//                     
//                     logger.LogInformation(
//                         "Step 3/{TotalSteps}: updating outbox message {MessageId} of type {MessageType} as processed ...",
//                         _totalSteps,
//                         outboxMessage.Id,
//                         outboxMessage.Type);
//                     
//                     await UpdateOutboxMessageAsync(outboxMessage, exception, ct);
//                 }
//
//                 logger.LogInformation("Step 4/{TotalSteps}: Committing transaction ...", _totalSteps);
//                 await transaction.CommitAsync(ct);
//                 logger.LogInformation("{Module} - Completed processing outbox messages", moduleConfig.ModuleName);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine(e);
//                 throw;
//             }
//         }, context.CancellationToken);
//     }
