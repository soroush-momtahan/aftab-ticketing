using Aftab.Shared.Application.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Aftab.Shared.Infrastructure.Inbox;

// public static class IntegrationEventHandlersFactory
#pragma warning disable S125
// {
#pragma warning restore S125
//     private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();
//
//     public static IEnumerable<IIntegrationEventHandler> GetHandlers(
//         Type type,
//         IServiceProvider serviceProvider,
//         Assembly assembly)
//     {
//         Type[] integrationEventHandlerTypes = HandlersDictionary.GetOrAdd(
//             $"{assembly.GetName().Name}-{type.Name}",
//             _ =>
//             {
//                 Type[] integrationEventHandlers = assembly.GetTypes()
//                     .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler<>).MakeGenericType(type)))
//                     .ToArray();
//
//                 return integrationEventHandlers;
//             });
//
//         List<IIntegrationEventHandler> handlers = [];
//         foreach (Type integrationEventHandlerType in integrationEventHandlerTypes)
//         {
//             object integrationEventHandler = serviceProvider.GetRequiredService(integrationEventHandlerType);
//
//             handlers.Add((integrationEventHandler as IIntegrationEventHandler)!);
//         }
//
//         return handlers;
//     }
// }

public static class IntegrationEventHandlersFactory
{
    public static IEnumerable<IIntegrationEventHandler> GetHandlers(
        Type domainEventType,
        IServiceProvider serviceProvider)
    {
        // 1. ساختن تایپ اینترفیس جنریک
        // مثال: IDomainEventHandler<ProductAddedEvent>
        Type handlerInterfaceType = typeof(IIntegrationEventHandler<>)
            .MakeGenericType(domainEventType);

        // 2. دریافت سرویس‌ها و کست کردن
        return serviceProvider
            .GetServices(handlerInterfaceType)
            .Cast<IIntegrationEventHandler>();
    }
}
