using Aftab.Shared.Domain.Errors;

namespace Aftab.Shared.Domain.Exceptions;

public class DomainValidationException(Error error) : Exception
{
    public Error Error { get; } = error;

}
