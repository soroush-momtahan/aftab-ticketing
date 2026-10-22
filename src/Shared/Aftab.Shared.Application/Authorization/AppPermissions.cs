namespace Aftab.Shared.Application.Authorization;

public static class AppPermissions
{
    public static class Tickets
    {
        public const string Create = "Tickets:create";
        public const string Read = "Tickets:read";
        public const string Update = "Tickets:update";
        public const string Delete = "Tickets:delete";
        public const string Reports = "Tickets:Reports";
    }
    public static class Organs
    {
        public const string Create = "Organs:create";
        public const string Read = "Organs:read";
        public const string Update = "Organs:update";
        public const string Delete = "Organs:delete";
    }
    public static class ReplyTickets
    {
        public const string Create = "ReplyTickets:create";
        public const string Read = "ReplyTickets:read";
        public const string Update = "ReplyTickets:update";
        public const string Delete = "ReplyTickets:delete";
    }

    // متدی برای گرفتن تمام پرمیشن‌ها (برای زمان Seeding به درد می‌خورد)
    public static IReadOnlyList<string> GetAll()
    {
        return typeof(AppPermissions)
            .GetNestedTypes()
            .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Select(f => f.GetValue(null)?.ToString())
            .Where(v => v != null)
            .Cast<string>()
            .ToList();
    }
}
