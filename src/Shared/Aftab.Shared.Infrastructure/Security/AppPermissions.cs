namespace Aftab.Shared.Infrastructure.Security;

public static class AppPermissions
{
    public static class Catalog
    {
        public const string Create = "catalog:create";
        public const string Read = "catalog:read";
        public const string Update = "catalog:update";
        public const string Delete = "catalog:delete";
    }

    public static class Orders
    {
        public const string View = "orders:view";
        public const string Manage = "orders:manage";
    }
    
    public static class Files
    {
        public const string ViewWholesalePrice = "files:view-whole-sale-price";
        public const string Create = "files:Create";
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
