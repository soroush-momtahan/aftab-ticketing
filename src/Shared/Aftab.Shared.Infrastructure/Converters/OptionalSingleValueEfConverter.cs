using Aftab.Shared.Domain.Optionals;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aftab.Shared.Infrastructure.Converters;

public class OptionalSingleValueEfConverter<TWrapper, TValue> : ValueConverter<TWrapper, TValue?>
    where TWrapper : IOptionalSingleValue<TWrapper, TValue> where TValue : class
{
    public OptionalSingleValueEfConverter() : base(
        v => ToDb(v),
        v => FromDb(v)
    )
    {
    }

    // تبدیل از Domain به Database
    private static TValue? ToDb(TWrapper? wrapper)
    {
        if (wrapper is null)
        {
            return default;
        }

        // اگر مقدار وجود داشت، آن را برگردان، وگرنه null (یا default)
        return wrapper.OptionalValue.TryGetValue(out TValue? value) ? value : default;
    }

    // تبدیل از Database به Domain
    private static TWrapper FromDb(TValue? value)
    {
        // نکته مهم: چک کردن null برای هر دو نوع Value Type (int?) و Reference Type (string?) کار می‌کند
        return value is null ? TWrapper.None : TWrapper.FromValue(value);
    }
}
