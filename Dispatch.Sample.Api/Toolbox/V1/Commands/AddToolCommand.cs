namespace Dispatch.Sample.Api.Toolbox.V1.Commands;

using Dispatch.Sample.Api.Toolbox.V1.Models;
using Dispatch.Sample.Api.Toolbox.V1.Services;
using Dispatch;

public record AddToolCommand(string Name) : ICommand<Tool>;

public sealed class AddToolCommandHandler(IToolRepository repo) : ICommandHandler<AddToolCommand, Tool>
{
    public Task<Tool> Handle(AddToolCommand request, CancellationToken ct)
        => Task.FromResult(repo.Add(request.Name));
}
