using System.Reflection;
using Aftab.Shared.Application.EventBus;
using Aftab.Shared.Infrastructure.Inbox;
using Aftab.Shared.Infrastructure.Inbox.OptionsSetups;
using Aftab.Shared.Infrastructure.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Aftab.Shared.Infrastructure.Extensions;

public static class InboxWorkerExtension
{
    public static IHostApplicationBuilder AddInboxWorkerConfiguration<TDbContext>(
        this IHostApplicationBuilder builder,
        string moduleName,
        Assembly assemblyToScanConsumers,
        Action<IRegistrationConfigurator> configureConsumers)
    where TDbContext : DbContext
    {
        builder.Services.TryAddSingleton<InsertOutboxMessageInterceptor>();
        builder
            .AddIntegrationEventHandlersWithIdempotency<TDbContext>(assemblyToScanConsumers)
            .AddConfigServices<TDbContext>(moduleName)
            .AddMessagingServices(configureConsumers)
            .AddQuartzService();
        return builder;
    }
    private static IHostApplicationBuilder AddIntegrationEventHandlersWithIdempotency<TDbContext>(
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
            // فیلتر کردن دقیق برای پیدا کردن IIntegrationEventHandler<>
            .Where(t => t.InterfaceType.IsGenericType &&
                        t.InterfaceType.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

        foreach (var handler in handlerTypes)
        {
            // 2. ثبت هندلر اصلی (Interface -> Implementation)
            builder.Services.TryAddScoped(handler.InterfaceType, handler.ImplementationType);

            // 3. ساختن دکوریتور
            // TEvent از اینترفیس هندلر
            Type eventType = handler.InterfaceType.GetGenericArguments()[0];
            
            // TDbContext از جنریک متد
            Type dbContextType = typeof(TDbContext);

            // ساختن IdempotentIntegrationEventHandler<TEvent, TDbContext>
            // نکته: کلاس IdempotentIntegrationEventHandler باید داخل همین پروژه Shared باشد
            Type decoratorType = typeof(IdempotentIntegrationEventHandler<,>)
                .MakeGenericType(eventType, dbContextType);

            // 4. دکوریت کردن
            builder.Services.Decorate(handler.InterfaceType, decoratorType);
        }

        return builder;
    }
    private static IHostApplicationBuilder AddConfigServices<TDbContext>(
        this IHostApplicationBuilder builder,
        string moduleName)
        where TDbContext : DbContext
    {
        builder.Services.AddSingleton(new InboxModuleConfig(moduleName));
        builder.Services.ConfigureOptions<InboxOptionsSetup>();
        builder.Services.ConfigureOptions<QuartzInboxOptionsSetup<TDbContext>>();
        return builder;
    }
    private static IHostApplicationBuilder AddMessagingServices(
        this IHostApplicationBuilder builder,
        Action<IRegistrationConfigurator> configureConsumers)
    {
        builder.Services.AddMassTransit(config =>
        {
            // سناریوی ۲ و ۳: اگر مصرف‌کننده‌ای وجود دارد، اینجا ثبت می‌شود
            configureConsumers.Invoke(config);

            config.SetKebabCaseEndpointNameFormatter();

            config.UsingRabbitMq((context, cfg) =>
            {
                IConfiguration configuration = context.GetRequiredService<IConfiguration>();
                string? connectionString = configuration.GetConnectionString("messaging");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Messaging connection string is missing.");
                }

                cfg.Host(new Uri(connectionString));
                
                // سناریوی ۲ و ۳: تنظیم خودکار نقاط انتهایی برای مصرف‌کننده‌ها
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
