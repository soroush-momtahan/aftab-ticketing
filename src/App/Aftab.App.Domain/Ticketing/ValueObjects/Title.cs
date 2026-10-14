using Aftab.App.Domain.Ticketing.Errors;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Domain.Ticketing.ValueObjects;

public record Title
{
    public const int MaxLenght = 128;
    public const int MinLenght = 3;
    public string Value { get; init; }
    
    private Title(string value) => Value = value;

    public static Result<Title> From(string title)
    {
        title = title.Trim();
        return title.Length switch
        {
            < MinLenght => Result.Failure<Title>(TicketingErrors.TitleIsTooShort),
            > MaxLenght => Result.Failure<Title>(TicketingErrors.TitleIsTooLong),
            _ => new Title(title)
        };
    }
}
