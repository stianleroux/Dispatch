namespace Dispatch.Sample.Api.Toolbox.V1.Commands;

using Dispatch;
using Dispatch.Sample.Api.Toolbox.V1.Services;

public record RemoveToolCommand(Guid Id) : ICommand<bool>;

public sealed class RemoveToolCommandHandler(IToolRepository repo) : ICommandHandler<RemoveToolCommand, bool>
{
    public Task<bool> Handle(RemoveToolCommand request, CancellationToken ct)
        => Task.FromResult(repo.Remove(request.Id));
}
