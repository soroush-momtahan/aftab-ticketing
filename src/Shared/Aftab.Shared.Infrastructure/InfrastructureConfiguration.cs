using Aftab.Shared.Infrastructure.Outbox;
using Aftab.Shared.Infrastructure.Security.Extenstions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Aftab.Shared.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IHostApplicationBuilder AddInfrastructureConfiguration(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.TryAddSingleton<InsertOutboxMessageInterceptor>();
        builder.Services.AddSecurityServices(configuration);
        builder.Services.AddAuthorization();
        return builder;
    }
}
