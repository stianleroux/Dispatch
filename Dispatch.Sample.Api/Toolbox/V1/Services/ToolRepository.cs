namespace Dispatch.Sample.Api.Toolbox.V1.Services;

using Dispatch.Sample.Api.Toolbox.V1.Models;

public interface IToolRepository
{
    Tool Add(string name);

    bool Remove(Guid id);

    Tool? Get(Guid id);

    IReadOnlyList<Tool> List();
}

public sealed class ToolRepository : IToolRepository
{
    private readonly Dictionary<Guid, Tool> store = [];

    public Tool Add(string name)
    {
        var tool = new Tool(Guid.NewGuid(), name);
        this.store[tool.Id] = tool;
        return tool;
    }

    public bool Remove(Guid id) => this.store.Remove(id);

    public Tool? Get(Guid id) => this.store.TryGetValue(id, out var t) ? t : null;

    public IReadOnlyList<Tool> List() => [.. this.store.Values];
}
