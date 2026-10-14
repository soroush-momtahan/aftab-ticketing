namespace Aftab.App.Application.Abstraction;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
