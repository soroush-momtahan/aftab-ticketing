using Aftab.Shared.Domain.Errors;

namespace Aftab.Shared.Domain.PrefixedGuidTools;

public static class PrefixedGuidErrors
{
    public static readonly Error EmptyInput = new(
        "PrefixedGuid.EmptyInput", 
        "ورودی اجباری است و نمی تواند شامل کاراکتر خالی باشد.",
        ErrorType.Validation);
    public static readonly Error MissingSeparator = new(
        "PrefixedGuid.MissingSeparator", 
        "ورودی حتی باید شامل کاراکتر '_' باشد",
        ErrorType.Validation);
    public static readonly Error InvalidPrefixFormat = new(
        "PrefixedGuid.InvalidPrefixFormat", 
        "پیشوند می تواند تنها شامل کاراتر های [a-z] باشد.",
        ErrorType.Validation);
    public static readonly Error WrongPrefix = new(
        "PrefixedGuid.WrongPrefix", 
        "پیشوند ورودی مطابق انتظار نیست.",
        ErrorType.Validation);
    public static readonly Error InvalidGuid = new(
        "PrefixedGuid.InvalidGuid", 
        "شناسه Guid معتبر نیست.",
        ErrorType.Validation);
    public static readonly Error EmptyGuid = new(
        "PrefixedGuid.EmptyGuid", 
        "شناسه Guid نمی تواند خالی باشد.",
        ErrorType.Validation);
}
