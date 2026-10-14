namespace Aftab.Shared.Infrastructure.Outbox.OptionsSetups;

public sealed class OutboxOptions
{
    public int IntervalInSeconds { get; init; }

    public int BatchSize { get; init; }
}
