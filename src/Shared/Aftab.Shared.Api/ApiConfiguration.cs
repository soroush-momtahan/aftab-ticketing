using System.Reflection;
using Aftab.Shared.Api.ConfigureOptions.ApiInfoOptions;
using Aftab.Shared.Api.ConfigureOptions.SwaggerOptions;
using Aftab.Shared.Api.Middlewares;
using Aftab.Shared.Infrastructure.Extensions;
using Aftab.Shared.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aftab.Shared.Api;

public static class ApiConfiguration
{
    public static IHostApplicationBuilder AddApiConfiguration(
        this IHostApplicationBuilder builder,
        params Assembly[] assembliesToScanEndpoints)
    {
        builder.AddServiceDefaults();
        
        IServiceCollection services = builder.Services;
        
        services.AddSwaggerServices();
        services.AddProblemHandlingServices();
        services.AddEndpoints(assembliesToScanEndpoints);
        
        return builder;
    }

    public static IApplicationBuilder UseApiConfiguration(this WebApplication app)
    {
        
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseHttpsRedirection();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapEndpoints();
        return app;
    }

    private static void AddSwaggerServices(this IServiceCollection services)
    {
        
        services.ConfigureOptions<ApiInfoOptionsSetup>();
        services.ConfigureOptions<SwaggerGenOptionsSetup>();
        services.ConfigureOptions<SwaggerUiOptionsSetup>();
        
        // for finding api's by swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddProblemHandlingServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }
}
