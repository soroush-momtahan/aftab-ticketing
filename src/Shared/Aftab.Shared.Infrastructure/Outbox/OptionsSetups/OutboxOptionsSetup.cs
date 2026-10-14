using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Aftab.Shared.Infrastructure.Outbox.OptionsSetups;

public class OutboxOptionsSetup(
    IConfiguration configuration) : IConfigureOptions<OutboxOptions>
{
    public void Configure(OutboxOptions options)
    {
        configuration.GetSection(nameof(OutboxOptions)).Bind(options);
    }
}
