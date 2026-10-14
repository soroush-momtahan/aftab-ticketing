using Aftab.Shared.Domain.Optionals;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aftab.Shared.Infrastructure.Converters;

public class OptionalPrefixedGuidEfConverter<TWrapper, TId>() : ValueConverter<TWrapper, Guid?>(
    v => ToDb(v),
    v => FromDb(v))
    where TWrapper : IOptionalPrefixedGuid<TWrapper, TId>
    where TId : PrefixedGuidV3
{
    // فقط ارجاع به متد استاتیک (بدون منطق پیچیده در اکسپرژن)

    // متد استاتیک معمولی: اینجا محدودیت Expression Tree وجود ندارد
    private static Guid? ToDb(TWrapper? wrapper)
    {
        if (wrapper is null)
        {
            return null;
        }

        // استخراج مقدار از Option
        return wrapper.OptionalValue.TryGetValue(out TId? id) ? id!.Value : null;
    }

    // متد استاتیک معمولی: اینجا می‌توانیم از static abstract interface members استفاده کنیم
    private static TWrapper FromDb(Guid? id)
    {
        if (id is null)
        {
            return TWrapper.None;
        }

        return TWrapper.FromGuid(id.Value);
    }
}
