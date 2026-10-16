using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Aftab.Shared.Api.ConfigureOptions.ApiInfoOptions;

public class ApiInfoOptionsSetup(IConfiguration configuration) : IConfigureOptions<ApiInfoOptions>
{
    public void Configure(ApiInfoOptions options)
    {
        configuration.GetSection(ApiInfoOptions.SectionName).Bind(options);
    }
}
