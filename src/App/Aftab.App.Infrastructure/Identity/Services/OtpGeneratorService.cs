using System.Globalization;
using System.Security.Cryptography;
using Aftab.App.Application.Identity.Services;

namespace Aftab.App.Infrastructure.Identity.Services;

internal sealed class OtpGeneratorService : IOtpGeneratorService
{
    public string GenerateSecureOtp()
    {
        // تولید یک عدد تصادفی 6 رقمی بین 100000 تا 999999
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString(CultureInfo.InvariantCulture);
    }
    
    public string GenerateMemorableOtp()
    {
        // برای امنیت بیشتر، می‌توانیم با احتمال 50 درصد کد معمولی تولید کنیم
        // و با احتمال 50 درصد کد خوش‌آهنگ تا قابل پیش‌بینی نباشد.
        bool usePattern = RandomNumberGenerator.GetInt32(0, 2) == 1;
        
        if (!usePattern)
        {
            return GenerateSecureOtp();
        }

        // انتخاب تصادفی یکی از سه الگوی خوش آهنگ: 1=AABB, 2=ABAB, 3=ABBA
        int patternType = RandomNumberGenerator.GetInt32(1, 4);

        // تولید عدد اول (A) بین 1 تا 9 (برای جلوگیری از شروع با صفر)
        int a = RandomNumberGenerator.GetInt32(1, 10);
        
        // تولید عدد دوم (B) بین 0 تا 9 به طوری که با A برابر نباشد
        int b;
        do
        {
            b = RandomNumberGenerator.GetInt32(0, 10);
        } while (a == b);

        // استفاده از Switch Expression (ویژگی‌های مدرن سی‌شارپ)
        return patternType switch
        {
            1 => $"{a}{a}{b}{b}", // مثال: 5544
            2 => $"{a}{b}{a}{b}", // مثال: 5454
            3 => $"{a}{b}{b}{a}", // مثال: 5445
            _ => GenerateSecureOtp() // Fallback
        };
    }

    public string GenerateMemorable6DigitOtp()
    {
        // برای جلوگیری از پیش‌بینی صددرصدی الگوریتم توسط هکر،
        // 30 درصد مواقع کدهای کاملا تصادفی تولید می‌کنیم (افزایش Entropy)
        if (RandomNumberGenerator.GetInt32(0, 100) < 30)
        {
            return GenerateSecureOtp();
        }

        // انتخاب یکی از 4 الگوی جذاب 6 رقمی
        int patternType = RandomNumberGenerator.GetInt32(1, 5);

        // تولید عدد اول (A) - نباید صفر باشد تا کد 6 رقمی بماند
        int a = RandomNumberGenerator.GetInt32(1, 10);
        
        // تولید عدد دوم (B) متمایز از A
        int b = GetDistinctNumber(a);
        
        // تولید عدد سوم (C) متمایز از A و B
        int c = GetDistinctNumber(a, b);

        return patternType switch
        {
            1 => $"{a}{a}{b}{b}{c}{c}", // الگوی جفتی: 442288
            2 => $"{a}{b}{c}{a}{b}{c}", // الگوی تکرار سه‌تایی: 715715
            3 => $"{a}{b}{c}{c}{b}{a}", // الگوی آینه‌ای (Palindrome): 832238
            4 => $"{a}{b}{b}{a}{a}{b}", // الگوی موزون: 644664
            _ => GenerateSecureOtp()
        };
    }

    /// <summary>
    /// متد کمکی برای تولید یک عدد تصادفی که با اعداد ورودی متفاوت باشد
    /// </summary>
    private static int GetDistinctNumber(params int[] excludeNumbers)
    {
        int number;
        do
        {
            number = RandomNumberGenerator.GetInt32(0, 10);
        } while (Array.IndexOf(excludeNumbers, number) != -1);

        return number;
    }
}
