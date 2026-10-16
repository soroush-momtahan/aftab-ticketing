using Aftab.App.Domain.People.Errors;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Domain.People.ValueObjects;
public record FullName
{
    public const int MinLenght = 3;
    public const int MaxLenght = 50;
    public string Firstname { get; init; }
    public string Lastname { get; init; }
    
    private FullName() {}

    public static Result<FullName> From(string firstname, string lastname)
    {
        firstname = firstname.Trim();
        lastname = lastname.Trim();
        List<Result<FullName>> results = [];
        switch (firstname.Length)
        {
            case < MinLenght:
                results.Add(Result.Failure<FullName>(PeopleErrors.FirstnameIsTooShort));
                break;
            case > MaxLenght:
                results.Add(Result.Failure<FullName>(PeopleErrors.FirstnameIsTooLong));
                break;
        }

        switch (lastname.Length)
        {
            case < MinLenght:
                results.Add(Result.Failure<FullName>(PeopleErrors.LastnameIsTooShort));
                break;
            case > MaxLenght:
                results.Add(Result.Failure<FullName>(PeopleErrors.LastnameIsTooLong));
                break;
        }

        if (results.Count > 0)
        {
            return Result.Combine(results);
        }

        return new FullName()
        {
            Firstname = firstname,
            Lastname = lastname
        };
    }
}
