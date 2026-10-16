namespace Aftab.Shared.Infrastructure.Security;

public static class AppPermissions
{
    public static class Tickets
    {
        public const string Create = "Tickets:create";
        public const string Read = "Tickets:read";
        public const string Update = "Tickets:update";
        public const string Delete = "Tickets:delete";
    }

    // متدی برای گرفتن تمام پرمیشن‌ها (برای زمان Seeding به درد می‌خورد)
    public static IReadOnlyList<string> GetAll()
    {
        return typeof(AppPermissions)
            .GetNestedTypes()
            .SelectMany(c => c.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy))
            .Select(f => f.GetValue(null)?.ToString())
            .Where(v => v != null)
            .Cast<string>()
            .ToList();
    }
}
