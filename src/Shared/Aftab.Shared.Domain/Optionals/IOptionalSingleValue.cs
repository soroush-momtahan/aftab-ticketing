namespace Aftab.Shared.Domain.Optionals;

// TWrapper: کلاس Wrapper شما (مثلاً OptionalDescription)
// TValue: نوع داده‌ای که در دیتابیس ذخیره می‌شود (مثلاً string, int, bool)
public interface IOptionalSingleValue<TWrapper, TValue>
    where TWrapper : IOptionalSingleValue<TWrapper, TValue> where TValue : class
{
    // 1. دسترسی به مقدار داخلی
    Option<TValue> OptionalValue { get; }

    // 2. ساخت Wrapper از مقدار دیتابیس (وقتی مقدار در دیتابیس Null نیست)
    static abstract TWrapper FromValue(TValue value);

    // 3. مقدار پیش‌فرض وقتی در دیتابیس Null است
    static abstract TWrapper None { get; }
}
