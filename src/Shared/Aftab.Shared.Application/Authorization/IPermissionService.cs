namespace Aftab.Shared.Application.Authorization;

public interface IPermissionService
{
    Task<HashSet<string>> GetPermissionsAsync(Guid memberId);
    Task<IReadOnlySet<string>> GetForRolesAsync(IEnumerable<string> roles, CancellationToken ct = default);
    Task RebuildCacheAsync(CancellationToken ct = default);
}
