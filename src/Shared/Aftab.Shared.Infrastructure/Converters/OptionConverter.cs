using Aftab.Shared.Domain.Optionals;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aftab.Shared.Infrastructure.Converters;

public class OptionConverter<T>() : ValueConverter<Option<T>, T?>(
    option => ToDb(option),
    value => FromDb(value)) where T : class
{
    private static T? ToDb(Option<T> option)
    {
        return option.TryGetValue(out T? value) ? value : null;
    }

    private static Option<T> FromDb(T? value)
    {
        return value == null ? Option<T>.None : Option<T>.Some(value);
    }
}
