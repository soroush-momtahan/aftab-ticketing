using Aftab.Shared.Domain.PrefixedGuidTools;

namespace Aftab.Shared.Domain.Optionals;

// TWrapper: کلاسی مثل OptionalParentId
// TId: کلاسی مثل CategoryId
public interface IOptionalPrefixedGuid<out TWrapper, TId>
    where TWrapper : IOptionalPrefixedGuid<TWrapper, TId>
    where TId : PrefixedGuidV3
{
    // 1. راهی برای خواندن مقدار
    Option<TId> OptionalValue { get; }

    // 2. راهی برای ساختن کلاس از یک Guid (برای وقتی که از دیتابیس می‌خوانیم)
    static abstract TWrapper FromGuid(Guid guid);

    // 3. راهی برای ساختن حالت خالی (برای وقتی که دیتابیس Null است)
    static abstract TWrapper None { get; }
}
