using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace Aftab.Shared.Infrastructure.Inbox.OptionsSetups;

internal sealed class QuartzInboxOptionsSetup<TDbContext>(IOptions<InboxOptions> inboxOptions)
    : IConfigureOptions<QuartzOptions>
    where TDbContext : DbContext
{
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;
    public void Configure(QuartzOptions options)
    {
        string jobKey = $"ProcessInboxJob-{typeof(TDbContext).Name}"; 
        var jobKeyIdentity = new JobKey(jobKey);
        options
            .AddJob<ProcessInboxJob<TDbContext>>(builder => builder.WithIdentity(jobKeyIdentity))
            .AddTrigger(builder =>
                builder
                    .ForJob(jobKeyIdentity)
                    .WithSimpleSchedule(schedule =>
                        schedule
                            .WithIntervalInSeconds(_inboxOptions.IntervalInSeconds)
                            .RepeatForever()));
    }
}
