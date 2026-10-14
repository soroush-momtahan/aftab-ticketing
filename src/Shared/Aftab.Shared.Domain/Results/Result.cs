using Aftab.Shared.Domain.Errors;

namespace Aftab.Shared.Domain.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("خطای تولید شده نامعتبر است", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success()
    {
        return new(true, Error.None);
    }
    public static Result Failure(Error error)
    {
        return new(false, error);
    }
    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new(value, true, Error.None);
    }
    public static Result<TValue> Failure<TValue>(Error error)
    {
        return new(default, false, error);
    }
    public static Result Combine(List<Result> results)
    {
        // تمام نتایج شکست خورده را پیدا کن
        var errors = results.Where(r => r.IsFailure).Select(r => r.Error).ToList();

        // اگر هیچ خطایی وجود نداشت، نتیجه موفقیت آمیز است
        if (errors.Count == 0)
        {
            throw new InvalidOperationException("دسترسی به مقدار نتیجه شکست خورده امکان پذیر نیست");
        }

        // اگر خطا وجود داشت، آنها را در یک خطای واحد ترکیب کن
        // (این فرض می‌کند که کلاس Error شما می‌تواند چندین خطا را مدیریت کند)
        return Failure(Error.Combine(errors.ToArray()));
    }
    public static Result<TValue> Combine<TValue>(List<Result<TValue>> results)
    {
        // تمام نتایج شکست خورده را پیدا کن
        var errors = results.Where(r => r.IsFailure).Select(r => r.Error).ToList();

        // اگر هیچ خطایی وجود نداشت، نتیجه موفقیت آمیز است
        if (errors.Count == 0)
        {
            throw new InvalidOperationException("دسترسی به مقدار نتیجه شکست خورده امکان پذیر نیست");
        }

        // اگر خطا وجود داشت، آنها را در یک خطای واحد ترکیب کن
        // (این فرض می‌کند که کلاس Error شما می‌تواند چندین خطا را مدیریت کند)
        return Failure<TValue>(Error.Combine(errors.ToArray()));
    }
}

public class Result<TValue> : Result
{
    [NotNull]
    public TValue Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("دسترسی به مقدار نتیجه شکست خورده امکان پذیر نیست");
    
    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    
    public static Result<TValue> ValidationFailure(Error error)
    {
        return new(default, false, error);
    }
}

