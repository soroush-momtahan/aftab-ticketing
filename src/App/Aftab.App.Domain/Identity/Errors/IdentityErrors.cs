using Aftab.App.Domain.People.Models;
using Aftab.Shared.Domain.Errors;

namespace Aftab.App.Domain.Identity.Errors;

public static class IdentityErrors
{
    public static readonly Error PasswordIsNotValid = new(
        "Identity.PasswordIsNotValid",
        $"پسورد وارد شده صحیح نمی باشد.",
        ErrorType.Validation);

    public static readonly Error PasswordIsTooShort = new(
        "Identity.PasswordIsTooShort",
        $"پسورد باید حداقل {Person.MinPasswordLength} کاراکتر باشد.",
        ErrorType.Validation);
    
    public static readonly Error PasswordIsTooLong = new(
        "Identity.PasswordIsTooLong",
        $"پسورد نباید بیشتر از {Person.MaxPasswordLength} کاراکتر باشد.",
        ErrorType.Validation);
    
    public static readonly Error PasswordDoesNotContainUppercaseLetter = new(
        "Identity.PasswordDoesNotContainUppercaseLetter",
        "رمز عبور باید شامل حداقل یک حرف بزرگ انگلیسی باشد.",
        ErrorType.Validation);
    
    public static readonly Error PasswordDoesNotContainLowercaseLetter = new(
        "Identity.PasswordDoesNotContainLowercaseLetter",
        "رمز عبور باید شامل حداقل یک حرف کوچک انگلیسی باشد.",
        ErrorType.Validation);
    
    public static readonly Error PasswordDoesNotContainNumber = new(
        "Identity.PasswordDoesNotContainNumber",
        "رمز عبور باید حداقل شامل یک عدد (0-9) باشد.",
        ErrorType.Validation);
    
    public static readonly Error PasswordDoesNotContainSpecialLetter = new(
        "Identity.PasswordDoesNotContainSpecialLetter",
        "رمز عبور باید شامل حداقل یک کاراکتر ویژه (مانند !@#$%) باشد.",
        ErrorType.Validation);
    
    public static readonly Error PasswordRequiresUniqueChars = new(
        "Identity.PasswordRequiresUniqueChars",
        $"رمز عبور شما باید حداقل دارای {Person.MinPasswordUniqueChars} کاراکتر متمایز باشد.",
        ErrorType.Validation);
    
    public static readonly Error ConfirmPasswordIsNotMatch = new(
        "Identity.ConfirmPasswordIsNotMatch",
        "تکرار رمز عبور با رمز عبور مطابقت ندارد.",
        ErrorType.Validation);
    
    public static readonly Error VerifyCodeIsNotValid = new(
        "Identity.VerifyCodeIsNotValid",
        $"کد احراز هویت معتبر نمی باشد.",
        ErrorType.Validation);
    public static readonly Error VerifyCodeIsEmpty = new(
        "Identity.VerifyCodeIsEmpty",
        $"کد احراز نمی تواند خالی باشد.",
        ErrorType.Validation);

    public static readonly Error SendOtpFailed = new(
        "Identity.SendOtpFailed",
        "ارسال ناموفق بود ، لطفا کمی صبر کنید و دوباره تلاش بفرمایید.",
        ErrorType.Validation);

    public static readonly Error TempVerifyTokenInvalid = new(
        "Identity.TempVerifyTokenInvalid",
        "توکن احراز هویت معتبر نیست",
        ErrorType.Validation);
}
