using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aftab.Shared.Api.Extensions;

public static class MigrationExtension
{
    public static void ApplyMigration<T>(this IApplicationBuilder app) where T : DbContext
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        ApplyMigration<T>(scope);
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        context.Database.Migrate();
    }
}
