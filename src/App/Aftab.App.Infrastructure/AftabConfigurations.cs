using Aftab.App.Application.Abstraction;
using Aftab.App.Application.Identity.Services;
using Aftab.App.Application.Ticketing.Services;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Infrastructure.Data;
using Aftab.App.Infrastructure.Data.ReadDb;
using Aftab.App.Infrastructure.Identity.ErrorDescriptor;
using Aftab.App.Infrastructure.Identity.Services;
using Aftab.App.Infrastructure.People.Repository;
using Aftab.App.Infrastructure.Ticketing.Repository;
using Aftab.App.Infrastructure.Ticketing.Services;
using Aftab.Shared.Infrastructure.Extensions;
using Aftab.Shared.Infrastructure.Outbox;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aftab.App.Infrastructure;

public static class AftabConfigurations
{
    public static IHostApplicationBuilder AddAftabServices(
        this IHostApplicationBuilder builder,
        string writeDbConnString,
        string readDbConnString,
        string cacheConnString)
    {
        builder
            .AddWriteDb(writeDbConnString)
            .AddReadDb(readDbConnString)
            .AddIdentityServices()
            .AddRedisCacheServices(cacheConnString);
            
        return builder;
    }
    private static IHostApplicationBuilder AddIdentityServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<OtpService>();
        builder.Services.AddSingleton<IOtpGeneratorService, OtpGeneratorService>();
        builder.Services.AddScoped<IOtpSenderService, OtpSenderService>();
        builder.AddAspIdentityServices();
        return builder;
    }
    private static IHostApplicationBuilder AddWriteDb(this IHostApplicationBuilder builder, string connString)
    {
        builder.Services.AddDbContext<AftabDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(connString));
            options.AddInterceptors(serviceProvider.GetRequiredService<InsertOutboxMessageInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<AftabDbContext>();
        builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();
        builder.Services.AddScoped<ITicketingRepository, TicketingRepository>();
        builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AftabDbContext>());
        return builder;
    }

    private static IHostApplicationBuilder AddReadDb(this IHostApplicationBuilder builder, string connString)
    {
        builder.AddNpgsqlDbContext<AftabReadDbContext>(connString);
        builder.Services.AddScoped<ITicketingQueryService, TicketingQueryService>();
        return builder;
    }
    private static void AddAspIdentityServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredUniqueChars =
                Person.MinPasswordUniqueChars; // کاربر باید حداقل ۴ کاراکتر متفاوت استفاده کند
            options.Password.RequiredLength = Person.MinPasswordLength;
        });

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AftabDbContext>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders();
        
        // 🔥 ترفند Best Practice برای API: جلوگیری از Redirect شدن به صفحات HTML
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
        
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
    }
}
