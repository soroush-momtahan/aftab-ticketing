using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Infrastructure.Organization.Configurations;
using Aftab.App.Infrastructure.People.Configurations;
using Aftab.App.Infrastructure.Ticketing.Configurations;
using Aftab.Shared.Infrastructure.Outbox.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Aftab.App.Infrastructure.Data;

public sealed class AftabDbContext(DbContextOptions<AftabDbContext> options) : IdentityDbContext<IdentityUser>(options), IUnitOfWork
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Organ> Organs => Set<Organ>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new TicketingConfiguration());
        builder.ApplyConfiguration(new PeopleConfiguration());
        builder.ApplyConfiguration(new OrganizationConfiguration());
        builder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        builder.ApplyConfiguration(new OutboxMessageConfiguration());
    }

    public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        IExecutionStrategy strategy = Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // اجرای کدهای بیزینسی تو (که از لایه اپلیکیشن پاس داده شده)
                await action();

                // کامیت کردن تراکنش
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                // رول‌بک در صورت بروز هرگونه خطا
                await transaction.RollbackAsync(cancellationToken);
                throw; // پاس دادن خطا به لایه‌های بالاتر
            }
        });
    }
}
