using Aftab.Shared.Domain.Errors;

namespace Aftab.Shared.Domain.Exceptions;

public sealed class AftabException(string requestName, Error? error = null, Exception? innerException = null)
    : Exception(error is not null ? error.Description : "خطای دامنه", innerException)
{
    public string RequestName { get; } = requestName;
    public Error? Error { get; } = error;
}
