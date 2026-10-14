namespace Aftab.Shared.Domain.Optionals;

public readonly struct Option<T> where T : class
{
    private readonly T _value;

    private readonly bool _hasValue;

    private Option(T value)
    {
        _value = value;
        _hasValue = true;
    }
    
    public bool TryGetValue(out T? value)
    {
        if (_hasValue)
        {
            value = _value;
            return true;
        }
        value = null;
        return false;
    }

    public T? GetValue => !_hasValue ? null : _value;

    public static Option<T> Some(T value) => new(value);
    public static Option<T> None => new();
    
    public static implicit operator Option<T>(T? value) => value is null ? None : Some(value);
}
