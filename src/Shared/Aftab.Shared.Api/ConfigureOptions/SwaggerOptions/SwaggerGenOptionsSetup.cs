using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
// ⚠️ دقت کن: پسوند Models حذف شده است و فقط همین Namespace کافیست

namespace Aftab.Shared.Api.ConfigureOptions.SwaggerOptions;

public class SwaggerGenOptionsSetup(IOptions<ApiInfoOptions.ApiInfoOptions> apiInfoOptions) : IConfigureOptions<SwaggerGenOptions>
{
    private ApiInfoOptions.ApiInfoOptions ApiInfoOptions { get; } = apiInfoOptions.Value;

    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc(ApiInfoOptions.Version, new OpenApiInfo
        {
            Version = ApiInfoOptions.Version,
            Title = ApiInfoOptions.Title,
            Description = ApiInfoOptions.Description,
            TermsOfService = !string.IsNullOrEmpty(ApiInfoOptions.TermsOfServiceUrl)
                ? new Uri(ApiInfoOptions.TermsOfServiceUrl)
                : null,
            Contact = ApiInfoOptions.Contact is not null
                ? new OpenApiContact
                {
                    Name = ApiInfoOptions.Contact.Name,
                    Url = new Uri(ApiInfoOptions.Contact.Url ?? "")
                }
                : null,
            License = ApiInfoOptions.License is not null
                ? new OpenApiLicense
                {
                    Name = ApiInfoOptions.License.Name,
                    Url = new Uri(ApiInfoOptions.License.Url ?? "")
                }
                : null
        });
        
        // ۱. تعریف نحوه دریافت توکن 
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "لطفاً توکن JWT خود را اینجا وارد کنید (نیاز به تایپ کلمه Bearer نیست)",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer" 
        });
        
        // ۲. 🚀 سینتکس جدید و بسیار تمیز برای دات‌نت ۱۰
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            // استفاده از Collection Expression در C# (همان آرایه خالی)
            [new OpenApiSecuritySchemeReference("Bearer", document)] = [] 
        });
    }
}
