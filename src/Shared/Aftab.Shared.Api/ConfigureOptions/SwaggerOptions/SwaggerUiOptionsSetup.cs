using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Aftab.Shared.Api.ConfigureOptions.SwaggerOptions;

public class SwaggerUiOptionsSetup(IOptions<ApiInfoOptions.ApiInfoOptions> options) : IConfigureOptions<SwaggerUIOptions>
{
    private ApiInfoOptions.ApiInfoOptions ApiInfoOptions { get; } = options.Value;

    public void Configure(SwaggerUIOptions options)
    {
        string version = ApiInfoOptions.Version;
        string title = ApiInfoOptions.Title;
        
        options.SwaggerEndpoint(
            $"/swagger/{version}/swagger.json",
            $"{title} {version}");
    }
}
