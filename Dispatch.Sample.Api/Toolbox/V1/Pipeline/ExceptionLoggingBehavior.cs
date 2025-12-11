namespace Dispatch.Sample.Api.Toolbox.V1.Pipeline;

using Dispatch;
using Microsoft.Extensions.Logging;

/// <summary>
/// Pipeline behavior that logs exceptions thrown by handlers.
/// Logs the exception details and re-throws to maintain standard exception flow.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
public sealed class ExceptionLoggingBehavior<TRequest, TResult>(ILogger<ExceptionLoggingBehavior<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult>
{
    public async Task<TResult> Handle(TRequest request, Func<Task<TResult>> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(ex, "[Pipeline] Exception in {RequestName}: {Message}", requestName, ex.Message);
            }
            throw;
        }
    }
}
