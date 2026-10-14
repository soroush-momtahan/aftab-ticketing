using Serilog.Context;

namespace Aftab.Shared.Application.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string moduleName = GetModuleName(typeof(TRequest).FullName!);
        string requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("Module", moduleName))
        {
#pragma warning disable S125
            // logger.LogInformation("Processing request {RequestName}", requestName);

            TResponse result = await next(cancellationToken);

            if (result.IsSuccess)
            {
                // logger.LogInformation("Completed request {RequestName}", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    logger.LogError("Completed request {RequestName} with error", requestName);
                }
            }

            return result;
        }
    }

    private static string GetModuleName(string requestName)
    {
        return requestName.Split('.')[2];
    }
}
