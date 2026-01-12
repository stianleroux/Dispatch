namespace Dispatch.Sample.Api.Toolbox.V1.Pipeline;

using System.Diagnostics;
using Dispatch;
using Microsoft.Extensions.Logging;

/// <summary>
/// Pipeline behavior that logs performance warnings when request execution exceeds 200ms.
/// Useful for identifying slow handlers that may need optimization.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
public sealed class PerformanceLoggingBehavior<TRequest, TResult>(ILogger<PerformanceLoggingBehavior<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult>
{
    private const int PerformanceThresholdMs = 200;

    public async Task<TResult> Handle(TRequest request, Func<Task<TResult>> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await next();
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs > PerformanceThresholdMs)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "[Pipeline] Performance: {RequestName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms)",
                        requestName,
                        elapsedMs,
                        PerformanceThresholdMs);
                }
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug(
                        "[Pipeline] Performance: {RequestName} completed in {ElapsedMs}ms",
                        requestName,
                        elapsedMs);
                }
            }
        }
    }
}
