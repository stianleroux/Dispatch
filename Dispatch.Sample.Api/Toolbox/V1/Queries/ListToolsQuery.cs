namespace Dispatch.Sample.Api.Toolbox.V1.Queries;

using Dispatch;
using Dispatch.Sample.Api.Toolbox.V1.Models;
using Dispatch.Sample.Api.Toolbox.V1.Services;

public record ListToolsQuery() : IQuery<IReadOnlyList<Tool>>;

public sealed class ListToolsQueryHandler(IToolRepository repo) : IQueryHandler<ListToolsQuery, IReadOnlyList<Tool>>
{
    public Task<IReadOnlyList<Tool>> Handle(ListToolsQuery request, CancellationToken ct)
        => Task.FromResult(repo.List());
}
