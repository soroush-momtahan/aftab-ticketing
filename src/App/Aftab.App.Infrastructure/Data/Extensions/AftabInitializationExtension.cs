using System.Security.Claims;
using Aftab.App.Infrastructure.Data.ReadDb;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Application.Caching;
using Aftab.Shared.Infrastructure.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aftab.App.Infrastructure.Data.Extensions;

public static class AftabInitializationExtension
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AppInitialization");

        try
        {
            // ۱. اعمال Migration ها
            logger.LogInformation("Applying database migrations...");
            AftabDbContext writeDbContext = serviceProvider.GetRequiredService<AftabDbContext>();
            AftabReadDbContext readDbContext = serviceProvider.GetRequiredService<AftabReadDbContext>();
            await writeDbContext.Database.MigrateAsync();
            await readDbContext.Database.MigrateAsync();

            // ۲. سید کردن نقش‌ها و پرمیشن‌ها
            logger.LogInformation("Seeding roles and permissions...");
            await SeedRolesAndPermissionsAsync(serviceProvider, logger);
            
            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error occurred during database initialization.");
            throw; // میخوایم اگر دیتابیس بالا نیومد، اپلیکیشن هم متوقف بشه
        }
    }
    private static async Task SeedRolesAndPermissionsAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        ICacheService cacheService = serviceProvider.GetRequiredService<ICacheService>();

        // ۱. گرفتن تمام پرمیشن‌های موجود در سیستم
        IReadOnlyList<string> allSystemPermissions = AppPermissions.GetAll();

        // ۲. تعریف نقشه (Map) پرمیشن‌های پیش‌فرض برای هر نقش
        // اینجا دقیقاً مشخص می‌کنی هر نقش چه دسترسی‌هایی داشته باشد
        var defaultRolePermissions = new Dictionary<string, List<string>>
        {
            { 
                "Agent", 
                allSystemPermissions.ToList()
            },
            { 
                "Client", 
                new List<string> ()
            }
        };

        var rolePermissionsDictForCache = new Dictionary<string, List<string>>();

        // ۳. بررسی و همگام‌سازی نقش‌ها و پرمیشن‌ها با دیتابیس
        foreach (KeyValuePair<string, List<string>> kvp in defaultRolePermissions)
        {
            string roleName = kvp.Key;
            List<string> expectedPermissions = kvp.Value;

            // ساخت نقش اگر وجود نداشت
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                logger.LogInformation("Role '{RoleName}' created.", roleName);
            }

            IdentityRole? role = await roleManager.FindByNameAsync(roleName);
            IList<Claim> existingClaims = await roleManager.GetClaimsAsync(role!);
            
            // استخراج پرمیشن‌هایی که الان در دیتابیس برای این نقش ثبت شده است
            var existingPermissions = existingClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            // الف) پیدا کردن پرمیشن‌های جدیدی که به کد اضافه شده اما در دیتابیس نیستند
            var missingPermissions = expectedPermissions.Except(existingPermissions).ToList();
            foreach (string permission in missingPermissions)
            {
                await roleManager.AddClaimAsync(role!, new Claim("permission", permission));
                logger.LogInformation("Added permission '{Permission}' to role '{RoleName}'.", permission, roleName);
            }

            // ب) (اختیاری) پیدا کردن پرمیشن‌هایی که از کد حذف شده‌اند و باید از دیتابیس هم حذف شوند
            // اگر می‌خواهی همیشه کد منبع حقیقت (Source of Truth) باشد، این بخش را فعال کن
            var obsoletePermissions = existingPermissions.Except(expectedPermissions).ToList();
            foreach (string permission in obsoletePermissions)
            {
                Claim claimToRemove = existingClaims.First(c => c.Type == "permission" && c.Value == permission);
                await roleManager.RemoveClaimAsync(role!, claimToRemove);
                logger.LogWarning("Removed obsolete permission '{Permission}' from role '{RoleName}'.", permission, roleName);
            }

            // آماده‌سازی برای کش شدن (لیست نهایی پرمیشن‌های مورد انتظار را در کش می‌گذاریم)
            rolePermissionsDictForCache[roleName] = expectedPermissions;
        }

        // ۴. پوش کردن دیتا به ردیس/کش در زمان استارت آپ
        await cacheService.SetAsync(CacheKeys.RolePermissions, rolePermissionsDictForCache);
        logger.LogInformation("Role-Permissions successfully pushed to Cache/Redis.");
    }
}
