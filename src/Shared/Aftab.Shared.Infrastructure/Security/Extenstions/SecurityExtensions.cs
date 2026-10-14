#pragma warning disable S125
// using System.Security.Claims;
#pragma warning restore S125
// using System.Text;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.IdentityModel.Tokens;
// using Otex.Shared.Infrastructure.Security.ConfigureOptions.JwtOptions;
//
// namespace Otex.Shared.Infrastructure.Security.Extenstions;
//
// public static class SecurityExtensions
// {
//     public static IServiceCollection AddSecurityServices(
//         this IServiceCollection services,
//         IConfiguration configuration)
//     {
//         services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
//         services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
//         services.AddJwtServices(configuration);
//         return services;
//     }
//
//     private static IServiceCollection AddJwtServices(
//         this IServiceCollection services, IConfiguration configuration)
//     {
//         // ۱. بایند کردن تنظیمات JwtOptions
//         services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
//         
//         JwtOptions jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
//
//         // ۲. تنظیمات Authentication و JWT
//         services.AddAuthentication(options =>
//             {
//                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                 options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//             })
//             .AddJwtBearer(options =>
//             {
//                 options.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuer = true,
//                     ValidateAudience = true,
//                     ValidateLifetime = true,
//                     ValidateIssuerSigningKey = true,
//                     ValidIssuer = jwtOptions.Issuer,
//                     ValidAudience = jwtOptions.Audience,
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
//                     ClockSkew = TimeSpan.Zero, // حذف ارفاق ۵ دقیقه‌ای پیش‌فرض
//                 
//                     // معرفی نام‌های استاندارد برای سیستم
//                     RoleClaimType = ClaimTypes.Role,
//                     NameClaimType = ClaimTypes.Name
//                 };
//
//                 // هندلر مدرن را به میدلور معرفی می‌کنیم (فقط برای اطمینان بیشتر، هرچند در نسخه‌های جدید دیفالت است)
//                 options.UseSecurityTokenValidators = false;
//                 
//                 // 🚀 این بخش را فقط برای عیب‌یابی اضافه کن
//                 options.Events = new JwtBearerEvents
//                 {
//                     OnAuthenticationFailed = context =>
//                     {
//                         // اگر توکن مشکلی داشته باشد، اینجا در کنسول چاپ می‌شود
//                         Console.ForegroundColor = ConsoleColor.Red;
//                         Console.WriteLine($"\n 🚨 JWT Validation Failed: {context.Exception.Message} \n");
//                         Console.ResetColor();
//                         return Task.CompletedTask;
//                     },
//                     OnChallenge = context =>
//                     {
//                         if (context.AuthenticateFailure != null)
//                         {
//                             Console.ForegroundColor = ConsoleColor.Yellow;
//                             Console.WriteLine($"\n ⚠️ JWT Challenge Triggered: {context.AuthenticateFailure.Message} \n");
//                             Console.ResetColor();
//                         }
//                         return Task.CompletedTask;
//                     }
//                 };
//             });
//         
//    
//
//         return services;
//     }
// }

using System.Security.Claims;
using System.Text;
using Aftab.Shared.Infrastructure.Security.ConfigureOptions.JwtOptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Aftab.Shared.Infrastructure.Security.Extenstions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddJwtServices(configuration);
        return services;
    }

    private static IServiceCollection AddJwtServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        JwtOptions jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };

                options.UseSecurityTokenValidators = false;

                // 🔍 ارسال مستقیم علت خطای 401 در خروجی Postman/Swagger
                // options.Events = new JwtBearerEvents
#pragma warning disable S125
                // {
#pragma warning restore S125
                //     OnChallenge = async context =>
                //     {
                //         // جلوی رفتار پیش‌فرض (پاسخ خالی 401) را می‌گیریم
                //         context.HandleResponse();
                //
                //         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //         context.Response.ContentType = "application/json";
                //
                //         string authHeader = context.Request.Headers.Authorization.ToString();
                //
                //         await context.Response.WriteAsJsonAsync(new
                //         {
                //             Step = "JWT OnChallenge",
                //             HasAuthorizationHeader = !string.IsNullOrEmpty(authHeader),
                //             ReceivedHeader = authHeader, // ببینیم اصلاً هدر به سرور رسیده یا نه!
                //             context.Error,
                //             context.ErrorDescription,
                //             ExceptionMessage = context.AuthenticateFailure?.Message ?? "توکن اصلاً پردازش نشد (احتمالاً هدر خالی است یا فرمت Bearer ندارد)"
                //         });
                //     }
                // };
            });

        // 🚀 ترفند طلایی: PostConfigure همیشه بعد از AddIdentity اجرا می‌شود 
        // و تضمین می‌کند هیچ چیزی نتواند تنظیمات JWT را بازنویسی کند!
        services.PostConfigure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        return services;
    }
}
