using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aftab.Shared.Infrastructure.Migrator;

public class DatabaseMigratorService<TDbContext>(
    IServiceProvider serviceProvider,
    ILogger<DatabaseMigratorService<TDbContext>> logger)
    where TDbContext : DbContext
{

    public async Task MigrateAsync(CancellationToken stoppingToken)
    {
        using Activity? activity = MigrationDiagnostics.ActivitySource.StartActivity(
            $"Migrating {typeof(TDbContext).Name}", 
            ActivityKind.Client);

        try
        {
            activity?.SetTag("db.context", typeof(TDbContext).Name);
            activity?.SetTag("db.system", "mssql");
            
            using IServiceScope scope = serviceProvider.CreateScope();
            
            TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            logger.LogInformation("Connecting to database for context {ContextName}...", typeof(TDbContext).Name);
            
            await RunMigrationAsync(dbContext, stoppingToken);

            logger.LogInformation("Migration for {ContextName} completed successfully.", typeof(TDbContext).Name);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            logger.LogError(ex, "An error occurred during migration of {ContextName}.", typeof(TDbContext).Name);
            throw new Exception("An error occurred during database migration.", ex);
        }
    }

    private async Task RunMigrationAsync(TDbContext dbContext, CancellationToken cancellationToken)
    {
        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            logger.LogInformation("Applying migrations for {ContextName}...", typeof(TDbContext).Name);
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }
}
