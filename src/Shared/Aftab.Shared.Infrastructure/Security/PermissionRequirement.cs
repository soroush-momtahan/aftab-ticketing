using System.Security.Claims;
using Aftab.Shared.Application.Caching;
using Aftab.Shared.Infrastructure.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aftab.Shared.Infrastructure.Security;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public class PermissionAuthorizationHandler(
    ICacheService cacheService,
    IServiceScopeFactory scopeFactory) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return;
        }

        // ۱. گرفتن نقش‌ها (در مرحله JWT بهت یاد میدم چطور کانفیگ کنی که فقط همون ClaimTypes.Role کافی باشه و درگیر نام‌گذاری‌های عجیب نشی)
        var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        
        if (!userRoles.Any())
        {
            return;
        }

        // ۲. خواندن از ردیس
        Dictionary<string, List<string>> rolePermissionsCache = await cacheService.GetAsync<Dictionary<string, List<string>>>(CacheKeys.RolePermissions) ??
        // ۳. اگر کش خالی بود (ردیس ریستارت شده بود)، Fallback می‌زنیم به دیتابیس!
        await ReloadPermissionsFromDatabaseAsync();                                                        
        // ۴. بررسی دسترسی با یک کد خوانا و تمیز
        bool hasPermission = userRoles.Any(role => 
            rolePermissionsCache.TryGetValue(role, out List<string>? permissions) && 
            permissions.Contains(requirement.Permission));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }

    // متد کمکی برای خواندن از دیتابیس در زمان قطعی کش
    private async Task<Dictionary<string, List<string>>> ReloadPermissionsFromDatabaseAsync()
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var rolePermissionsDict = new Dictionary<string, List<string>>();

        var roles = roleManager.Roles.ToList();
        foreach (IdentityRole role in roles)
        {
            IList<Claim> claims = await roleManager.GetClaimsAsync(role);
            rolePermissionsDict[role.Name!] = claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();
        }

        // کش رو دوباره پر می‌کنیم (بدون انقضا)
        await cacheService.SetAsync(CacheKeys.RolePermissions, rolePermissionsDict);
        
        return rolePermissionsDict;
    }
}
