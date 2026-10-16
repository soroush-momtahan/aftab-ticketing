using System.Reflection;
using Aftab.Shared.Application.EventBus;
using Aftab.Shared.Application.Messaging;
using Aftab.Shared.Infrastructure.Outbox;
using Aftab.Shared.Infrastructure.Outbox.OptionsSetups;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Aftab.Shared.Infrastructure.Extensions;

public static class OutboxWorkerExtension
{
    public static IHostApplicationBuilder AddOutboxWorkerConfiguration<TDbContext>(
        this IHostApplicationBuilder builder,
        string moduleName,
        Assembly assemblyToScan)
        where TDbContext : DbContext
    {
        builder
            .AddServiceDefaults()
            // .AddMessagingServices()
            .AddDomainEventHandlersWithIdempotency<TDbContext>(assemblyToScan)
            .AddConfigServices<TDbContext>(moduleName)
            .AddQuartzService();
        return builder;
    }
    private static IHostApplicationBuilder AddDomainEventHandlersWithIdempotency<TDbContext>(
        this IHostApplicationBuilder builder,
        Assembly assemblyToScan)
        where TDbContext : DbContext
    {
        // 1. پیدا کردن کلاس‌های هندلر
        var handlerTypes = assemblyToScan.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface)
            .SelectMany(type => type.GetInterfaces(), (impl, iface) => new
            {
                ImplementationType = impl,
                InterfaceType = iface
            })
            // فیلتر کردن دقیق برای پیدا کردن IDomainEventHandler<>
            .Where(t => t.InterfaceType.IsGenericType &&
                        t.InterfaceType.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

        // 2. گروه‌بندی بر اساس نوع اینترفیس (مثلاً تمام هندلرهای ProductAddedEvent در یک گروه قرار می‌گیرند)
        var groupedHandlers = handlerTypes.GroupBy(x => x.InterfaceType);
        
        foreach (var group in groupedHandlers)
        {
            Type interfaceType = group.Key;

            // 3. ثبت تمام هندلرهای مربوط به این ایونت (استفاده از AddScoped به جای TryAdd)
            foreach (var handler in group)
            {
                builder.Services.AddScoped(interfaceType, handler.ImplementationType);
            }

            // 3. ساختن دکوریتور
            // TEvent از اینترفیس هندلر
            Type eventType = interfaceType.GetGenericArguments()[0];
            
            // TDbContext از جنریک متد
            Type dbContextType = typeof(TDbContext);

            // ساختن IdempotentDomainEventHandler<TEvent, TDbContext>
            // نکته: کلاس IdempotentDomainEventHandler باید داخل همین پروژه Shared باشد
            Type decoratorType = typeof(IdempotentDomainEventHandler<,>)
                .MakeGenericType(eventType, dbContextType);

            // 4. دکوریت کردن
            builder.Services.Decorate(interfaceType, decoratorType);
        }

        return builder;
    }
    
    private static IHostApplicationBuilder AddConfigServices<TDbContext>(
        this IHostApplicationBuilder builder,
        string moduleName)
        where TDbContext : DbContext
    {
        builder.Services.AddSingleton(new OutboxModuleConfig(moduleName));
        builder.Services.ConfigureOptions<OutboxOptionsSetup>();
        builder.Services.ConfigureOptions<QuartzOutboxOptionsSetup<TDbContext>>();
        return builder;
    }

    private static IHostApplicationBuilder AddMessagingServices(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEventBus, EventBus.EventBus>();
        builder.Services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((context, cfg) =>
            {
                string? connectionString = context
                    .GetRequiredService<IConfiguration>()
                    .GetConnectionString("messaging");

                cfg.Host(new Uri(connectionString!));
                cfg.ConfigureEndpoints(context);
            });
        });
        return builder;
    }
    
    private static void AddQuartzService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}
