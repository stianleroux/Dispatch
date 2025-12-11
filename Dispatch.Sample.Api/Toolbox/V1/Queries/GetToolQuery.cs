namespace Dispatch.Sample.Api.Toolbox.V1.Queries;

using Dispatch;
using Dispatch.Sample.Api.Toolbox.V1.Models;
using Dispatch.Sample.Api.Toolbox.V1.Services;

public record GetToolQuery(Guid Id) : IQuery<Tool?>;

public sealed class GetToolQueryHandler(IToolRepository repo) : IQueryHandler<GetToolQuery, Tool?>
{
    public Task<Tool?> Handle(GetToolQuery request, CancellationToken ct) =>
        // Demo negative path: translate InvalidOperationException via exception handler
        request.Id == Guid.Empty
            ? throw new InvalidOperationException("Tool id cannot be empty")
            : Task.FromResult(repo.Get(request.Id));
}
