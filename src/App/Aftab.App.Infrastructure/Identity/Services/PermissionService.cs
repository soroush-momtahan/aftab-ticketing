using Aftab.App.Infrastructure.Data;
using Aftab.Shared.Application.Authorization;
using Aftab.Shared.Application.Caching;
using Aftab.Shared.Infrastructure.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Aftab.App.Infrastructure.Identity.Services;

public class PermissionService(
    ICacheService cache,
    AftabDbContext db) : IPermissionService
{
    public Task<HashSet<string>> GetPermissionsAsync(Guid memberId)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlySet<string>> GetForRolesAsync(IEnumerable<string> roles, CancellationToken ct = default)
    {
        Dictionary<string, string[]> map = await cache.GetAsync<Dictionary<string, string[]>>(CacheKeys.RolePermissions, ct)
                                           ?? await RebuildCoreAsync(ct);

        return roles.Where(map.ContainsKey).SelectMany(r => map[r]).ToHashSet();
    }

    public Task RebuildCacheAsync(CancellationToken ct = default) => RebuildCoreAsync(ct);

    private async Task<Dictionary<string, string[]>> RebuildCoreAsync(CancellationToken ct)
    {
        var rows = await (
            from rc in db.Set<IdentityRoleClaim<string>>()
            join r in db.Roles on rc.RoleId equals r.Id
            where rc.ClaimType == "permission"
            select new { r.Name, rc.ClaimValue }).ToListAsync(ct);

        var map = rows.GroupBy(x => x.Name!)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ClaimValue!).ToArray());

        await cache.SetAsync(CacheKeys.RolePermissions, map, null, ct);
        return map;
    }
}
