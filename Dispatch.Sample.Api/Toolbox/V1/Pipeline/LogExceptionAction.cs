namespace Dispatch.Sample.Api.Toolbox.V1.Pipeline;

using Dispatch;

public sealed class LogExceptionAction<TRequest> : IRequestExceptionAction<TRequest, Exception>
{
    public Task Execute(TRequest request, Exception exception, CancellationToken ct)
    {
        Console.Error.WriteLine($"[Toolbox] Exception: {exception.GetType().Name} - {exception.Message}");
        return Task.CompletedTask;
    }
}
