using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Aftab.Shared.Infrastructure.Inbox.OptionsSetups;

public class InboxOptionsSetup(IConfiguration configuration) : IConfigureOptions<InboxOptions>
{
    public void Configure(InboxOptions options)
    {
        configuration.GetSection(nameof(InboxOptions)).Bind(options);
    }
}
