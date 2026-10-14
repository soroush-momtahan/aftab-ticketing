namespace Aftab.Shared.Domain.Errors;

public record Error
{
    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Empty(string name)
    {
        return new(
            "General.Empty",
            $"فیلد {name} نمی تواند خلی باشد.",
            ErrorType.Validation
        );
    }

    public static readonly Error NullValue = new(
        "General.Null",
        "مقدار تهی شناسایی شد.",
        ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public static Error Failure(string code, string description)
    {
        return new(code, description, ErrorType.Failure);
    }

    public static Error NotFound(string code, string description)
    {
        return new(code, description, ErrorType.NotFound);
    }

    public static Error Problem(string code, string description)
    {
        return new(code, description, ErrorType.Problem);
    }

    public static Error Conflict(string code, string description)
    {
        return new(code, description, ErrorType.Conflict);
    }

    public static Error Validation(string code, string description)
    {
        return new(code, description, ErrorType.Validation);
    }

    // متد جدیدی که باید اضافه شود
    public static Error Combine(params Error[] errors)
    {
        if (errors.Length == 0)
        {
            return None;
        }

        if (errors.Length == 1)
        {
            return errors[0];
        }

        string combinedCode = string.Join(", ", errors.Select(e => e.Code));
        string combinedDescription = string.Join("\n", errors.Select(e => e.Description));

        return new Error(
            code: combinedCode,
            description: combinedDescription,
            type: ErrorType.Validation);
    }
};
