using Aftab.Shared.Domain.Exceptions;
using Aftab.Shared.Domain.Results;

namespace Aftab.Shared.Domain.PrefixedGuidTools;

public abstract record PrefixedGuidV3
{
    public Guid Value { get; }
    
    // کش کردن پرفیکس‌ها برای جلوگیری از کندی رفلکشن
    private static readonly ConcurrentDictionary<Type, string> PrefixCache = new();
    
    protected PrefixedGuidV3(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new AftabException(nameof(PrefixedGuidV3), PrefixedGuidErrors.EmptyGuid);
        }
        Value = value;
    }
    // در داخل کلاس abstract record PrefixedGuidV3 اضافه کنید

    public static bool TryGetPrefix(string input, out string? prefix)
    {
        prefix = null;

        // 1. بررسی‌های اولیه: خالی نبودن
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        // 2. پیدا کردن جداکننده (_)
        int separatorIndex = input.IndexOf('_');

        // اگر جداکننده پیدا نشد، یا در ابتدای رشته بود (_guid)، یا در انتها (prd_)
        if (separatorIndex <= 0 || separatorIndex == input.Length - 1)
        {
            return false;
        }

        // 3. اعتبارسنجی بخش GUID (بسیار مهم)
        // با استفاده از Span حافظه اضافی اشغال نمی‌کنیم (High Performance)
        ReadOnlySpan<char> guidSpan = input.AsSpan(separatorIndex + 1);
    
        if (!Guid.TryParse(guidSpan, out _))
        {
            // اگر بخش دوم یک Guid معتبر نبود، پس این یک PrefixedGuid نیست
            return false;
        }

        // 4. استخراج پرفیکس
        // فقط وقتی مطمئن شدیم فرمت درست است، زیررشته (Substring) می‌سازیم
        prefix = input.Substring(0, separatorIndex);

        return true;
    }
    
    public static T New<T>() where T : PrefixedGuidV3
    {
        var newGuid = Guid.CreateVersion7(); 
        return CreateInstance<T>(newGuid);
    }
    
    public static Result<T> From<T>(string input) where T : PrefixedGuidV3
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Result<T>.ValidationFailure(PrefixedGuidErrors.EmptyInput);
        }

        // جدا کردن پرفیکس و Guid
        string[] parts = input.Split('_');
        if (parts.Length != 2)
        {
            return Result<T>.ValidationFailure(PrefixedGuidErrors.MissingSeparator);
        }

        string prefix = parts[0];
        string guidString = parts[1];
        
        string expectedPrefix = GetPrefix(typeof(T));
        if (!string.Equals(prefix, expectedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Result<T>.ValidationFailure(PrefixedGuidErrors.WrongPrefix);
        }
        
        if (!Guid.TryParse(guidString, out Guid guidValue))
        {
            return Result<T>.ValidationFailure(PrefixedGuidErrors.InvalidGuid);
        }
        
        return CreateInstance<T>(guidValue);
    }

    public sealed override string ToString() => $"{GetPrefix(GetType())}_{Value}";

    // متد کمکی برای ساخت نمونه (Reflection بهینه‌تر)
    private static T CreateInstance<T>(Guid value) where T : PrefixedGuidV3
    {
        return (T)Activator.CreateInstance(typeof(T), value)!;
    }

    // دریافت پرفیکس با استفاده از کش
    public static string GetPrefix(Type type)
    {
        return PrefixCache.GetOrAdd(type, t =>
        {
            PrefixAttribute? attr = t.GetCustomAttribute<PrefixAttribute>();
            if (attr is null)
            {
                throw new InvalidOperationException($"Type '{t.Name}' is missing [Prefix] attribute.");
            }

            return attr.Prefix;
        });
    }
    
    public static implicit operator Guid(PrefixedGuidV3 prefixedGuid) => prefixedGuid.Value;
    public static implicit operator string(PrefixedGuidV3 prefixedGuid) => prefixedGuid.ToString();
}

