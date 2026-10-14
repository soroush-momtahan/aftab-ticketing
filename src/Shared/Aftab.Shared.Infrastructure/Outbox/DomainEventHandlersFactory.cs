using Aftab.Shared.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Aftab.Shared.Infrastructure.Outbox;



// public static class DomainEventHandlersFactory
#pragma warning disable S125
// {
#pragma warning restore S125
//     private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();
//
//     public static IEnumerable<IDomainEventHandler> GetHandlers(
//         Type type,
//         IServiceProvider serviceProvider,
//         Assembly assembly)
//     {
//         Type[] domainEventHandlerTypes = HandlersDictionary.GetOrAdd(
//             $"{assembly.GetName().Name}{type.Name}",
//             _ =>
//             {
//                 Type[] domainEventHandlerTypes = assembly.GetTypes()
//                     .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler<>).MakeGenericType(type)))
//                     .ToArray();
//
//                 return domainEventHandlerTypes;
//             });
//
//         List<IDomainEventHandler> handlers = [];
//         foreach (Type domainEventHandlerType in domainEventHandlerTypes)
//         {
//             object domainEventHandler = serviceProvider.GetRequiredService(domainEventHandlerType);
//
//             handlers.Add((domainEventHandler as IDomainEventHandler)!);
//         }
//
//         return handlers;
//     }
// }

// public static class DomainEventHandlersFactory
// {
//     public static IEnumerable<IDomainEventHandler> GetHandlers(
//         Type domainEventType,
//         IServiceProvider serviceProvider)
//     {
//         // 1. ساختن تایپ اینترفیس جنریک بر اساس نوع ایونت
//         // مثال: IDomainEventHandler<ProductAddedEvent>
//         Type handlerInterfaceType = typeof(IDomainEventHandler<>)
//             .MakeGenericType(domainEventType);
//
//         // 2. درخواست تمام هندلرهای ثبت شده برای این اینترفیس از DI
//         // نکته مهم: استفاده از GetServices (جمع) به جای GetService
//         IEnumerable<object?> handlers = serviceProvider.GetServices(handlerInterfaceType);
//
//         // 3. کست کردن و برگرداندن
//         foreach (object? handler in handlers)
//         {
//             yield return (IDomainEventHandler)handler;
//         }
//     }
// }

public static class DomainEventHandlersFactory
{
    public static IEnumerable<IDomainEventHandler> GetHandlers(
        Type domainEventType,
        IServiceProvider serviceProvider)
    {
        // 1. ساختن تایپ اینترفیس جنریک
        // مثال: IDomainEventHandler<ProductAddedEvent>
        Type handlerInterfaceType = typeof(IDomainEventHandler<>)
            .MakeGenericType(domainEventType);

        // 2. دریافت سرویس‌ها و کست کردن
        return serviceProvider
            .GetServices(handlerInterfaceType)
            .Cast<IDomainEventHandler>();
    }
}
