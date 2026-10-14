using Aftab.App.Domain.People.ValueObjects;
using Aftab.Shared.Domain.Errors;

namespace Aftab.App.Domain.People.Errors;

public static class PeopleErrors
{
    public static readonly Error FirstnameIsTooShort = new(
        "Identity.FirstnameIsTooShort",
        $"نام نمی تواند کمتر از {FullName.MinLenght} حرف باشد.",
        ErrorType.Validation);

    public static readonly Error PersonNotFound = new(
        "Identity.PersonNotFound",
        $"کاربر مورد نظر یافت نشد.",
        ErrorType.Validation);

    public static readonly Error FirstnameIsTooLong = new(
        "Identity.FirstnameIsTooLong",
        $"نام نمی تواند بیشتر از {FullName.MaxLenght} حرف باشد.",
        ErrorType.Validation);

    public static readonly Error FirstnameIsEmpty = new(
        "Identity.FirstnameIsEmpty",
        "وارد کردن نام الزامی است.",
        ErrorType.Validation);

    public static readonly Error LastnameIsTooShort = new(
        "Identity.LastnameIsTooShort",
        $"نام خانوادگی نمی تواند کمتر از {FullName.MinLenght} حرف باشد.",
        ErrorType.Validation);

    public static readonly Error LastnameIsTooLong = new(
        "Identity.LastnameIsTooLong",
        $"نام خانوادگی نمی تواند بیشتر از {FullName.MaxLenght} حرف باشد.",
        ErrorType.Validation);

    public static readonly Error LastnameIsEmpty = new(
        "Identity.LastnameIsEmpty",
        "وارد کردن نام خانوادگی الزامی است.",
        ErrorType.Validation);
    
    public static readonly Error MobileNotValid = new(
        "Identity.MobileNotValid",
        $"شماره وارد شده معتبر نمی باشد.",
        ErrorType.Validation);
    public static readonly Error MobileNotValidLenght = new(
        "Identity.MobileNotValidLenght",
        $"شماره موبایل باید دقیقا برابر با 11 رقم باشد.",
        ErrorType.Validation);

    public static Error DuplicateMobile(string mobile) => new(
        "Identity.DuplicateMobile",
        $"شماره موبایل '{mobile}' قبلا ثبت شده است.",
        ErrorType.Validation);
}
