namespace Dispatch.Sample.Api.Toolbox.V1.Pipeline;

using Dispatch;
using Dispatch.Sample.Api.Toolbox.V1.Models;

public sealed class TranslateInvalidOpHandler<TRequest> : IRequestExceptionHandler<TRequest, Tool, InvalidOperationException>
{
    public Task<Tool> Handle(TRequest request, InvalidOperationException exception, CancellationToken ct)
        => Task.FromResult(new Tool(Guid.Empty, "Unknown"));
}
