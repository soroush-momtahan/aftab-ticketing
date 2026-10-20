namespace Aftab.Shared.Application.Authorization;

public static class AppRoles
{
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string Admin = nameof(Admin);
    public const string Agent = nameof(Agent);
}

public static class DefaultRolePermissions
{
    public static IReadOnlyDictionary<string, string[]> Map { get; } = new Dictionary<string, string[]>
    {
        [AppRoles.SuperAdmin] = [.. AppPermissions.GetAll()],
        [AppRoles.Agent] = [.. AppPermissions.GetAll()],
        [AppRoles.Admin] =
        [
            AppPermissions.Tickets.Delete
        ]
    };
}
