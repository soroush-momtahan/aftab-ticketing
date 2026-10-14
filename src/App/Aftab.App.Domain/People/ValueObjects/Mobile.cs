using Aftab.App.Domain.People.Errors;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Domain.People.ValueObjects;

public record Mobile
{
    public const int ValidLenght = 11;
    public string Value { get; init; }
    private Mobile() {}

    public static Result<Mobile> From(string mobile)
    {
        mobile = mobile.Trim();
        if (string.IsNullOrWhiteSpace(mobile) || mobile.Length != ValidLenght)
        {
            return Result.Failure<Mobile>(PeopleErrors.MobileNotValid);
        }

        return new Mobile()
        {
            Value = mobile
        };
    }
}
