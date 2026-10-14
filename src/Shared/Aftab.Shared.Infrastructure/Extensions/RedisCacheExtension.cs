using Aftab.Shared.Application.Caching;
using Aftab.Shared.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Aftab.Shared.Infrastructure.Extensions;

public static class RedisCacheExtension
{
    public static IHostApplicationBuilder AddRedisCacheServices(
        this IHostApplicationBuilder builder,
        string cacheName)
    {
        builder.Services.TryAddSingleton<ICacheService, CacheService>();

        builder.AddKeyedRedisDistributedCache(cacheName);

        return builder;
    }
}
