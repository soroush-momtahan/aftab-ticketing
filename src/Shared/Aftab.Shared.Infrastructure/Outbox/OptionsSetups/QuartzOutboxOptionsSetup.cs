using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace Aftab.Shared.Infrastructure.Outbox.OptionsSetups;

internal sealed class QuartzOutboxOptionsSetup<TDbContext>(IOptions<OutboxOptions> outboxOptions) 
    : IConfigureOptions<QuartzOptions>
    where TDbContext : DbContext
{
    private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobKey = $"ProcessOutboxJob-{typeof(TDbContext).Name}"; 
        
        var jobKeyIdentity = new JobKey(jobKey);

        options
            .AddJob<ProcessOutboxJob<TDbContext>>(builder => builder.WithIdentity(jobKeyIdentity))
            .AddTrigger(builder =>
                builder
                    .ForJob(jobKeyIdentity)
                    .WithSimpleSchedule(schedule =>
                        schedule
                            .WithIntervalInSeconds(_outboxOptions.IntervalInSeconds)
                            .RepeatForever()));
    }
}
