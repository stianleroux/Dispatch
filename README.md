# Dispatch

[![Build](https://img.shields.io/github/actions/workflow/status/StianLeRoux/Dispatch/dotnet.yml?branch=main)](https://github.com/StianLeRoux/Dispatch/actions)
[![NuGet](https://img.shields.io/nuget/v/Dispatch.svg)](https://www.nuget.org/packages/Dispatch/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Dispatch.svg)](https://www.nuget.org/packages/Dispatch/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Dispatch is a lightweight CQRS-style dispatcher for .NET. It centralizes command, query, and notification handling, with optional pipeline behaviors and exception boundaries to keep handlers focused and resilient.
Special thanks to the excellent [Scrutor](https://github.com/khellang/Scrutor) library used for assembly scanning and registration.

## Minimal API Example

The sample project [Dispatch.Sample.Api](Dispatch.Sample.Api) wires `Dispatcher` with commands, queries, notifications, and exception behaviors.

In Development, the sample exposes API docs at `/scalar/index.html` (Scalar UI) and the OpenAPI JSON at `/openapi/v1.json`. The root `/` redirects to the Scalar UI.

Program setup:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDispatcher(cfg => cfg.RegisterServicesFromAssembly(typeof(AddToolCommandHandler).Assembly));

var app = builder.Build();

app.MapPost("/tools", async (AddToolCommand cmd, Dispatcher d, CancellationToken ct)
    => await d.Send<AddToolCommand, Tool>(cmd, ct));

app.MapDelete("/tools/{id}", async (Guid id, Dispatcher d, CancellationToken ct)
    => await d.Send<RemoveToolCommand, bool>(new RemoveToolCommand(id), ct));

app.MapGet("/tools/{id}", async (Guid id, Dispatcher d, CancellationToken ct)
    => await d.Query<GetToolQuery, Tool?>(new GetToolQuery(id), ct));

app.MapGet("/tools", async (Dispatcher d, CancellationToken ct)
    => await d.Query<ListToolsQuery, IReadOnlyList<Tool>>(new ListToolsQuery(), ct));

app.Run();
```

Stian’s Toolbox domain:

```csharp
public record Tool(Guid Id, string Name);

public record AddToolCommand(string Name) : ICommand<Tool>;
public class AddToolCommandHandler : ICommandHandler<AddToolCommand, Tool>
{
    public Task<Tool> Handle(AddToolCommand request, CancellationToken ct)
        => Task.FromResult(new Tool(Guid.NewGuid(), request.Name));
}

public record RemoveToolCommand(Guid Id) : ICommand<bool>;
public class RemoveToolCommandHandler : ICommandHandler<RemoveToolCommand, bool>
{
    public Task<bool> Handle(RemoveToolCommand request, CancellationToken ct)
        => Task.FromResult(true);
}

public record GetToolQuery(Guid Id) : IQuery<Tool?>;
public class GetToolQueryHandler : IQueryHandler<GetToolQuery, Tool?>
{
    public Task<Tool?> Handle(GetToolQuery request, CancellationToken ct)
        => Task.FromResult<Tool?>(null);
}

public record ListToolsQuery() : IQuery<IReadOnlyList<Tool>>;
public class ListToolsQueryHandler : IQueryHandler<ListToolsQuery, IReadOnlyList<Tool>>
{
    public Task<IReadOnlyList<Tool>> Handle(ListToolsQuery request, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Tool>>(Array.Empty<Tool>());
}
```

## Dispatcher Quick Reference

- Commands: implement `ICommand<TResult>` + `ICommandHandler<TCommand, TResult>` and call `dispatcher.Send(...)`
- Queries: implement `IQuery<TResult>` + `IQueryHandler<TQuery, TResult>` and call `dispatcher.Query(...)`
- Notifications: implement `INotification` + `INotificationHandler<TNotification>` and call `dispatcher.Publish(...)`
- Pipelines: implement `IPipelineBehavior<TRequest, TResult>` to wrap handler execution (logging, validation, etc.)
- Exception boundaries: add `IRequestExceptionAction<TRequest, TException>` for side-effects and `IRequestExceptionHandler<TRequest, TResult, TException>` to translate exceptions into results

### ExecutePipeline

`Dispatcher` internally composes registered `IPipelineBehavior<TRequest, TResult>` instances into a chain and executes the handler through that chain. If an exception is thrown, it will:
- execute all matching `IRequestExceptionAction<TRequest, Exception>` (sorted by priority)
- try a single `IRequestExceptionHandler<TRequest, TResult, Exception>` to produce a safe `TResult`
- rethrow if no handler is available

This keeps handlers clean while allowing cross-cutting concerns like logging, validation, and error translation.

## Exception Boundaries

- Actions: run for thrown exceptions; multiple actions may execute (sorted by priority)
- Handlers: only one may handle and return a result; if none, the exception is rethrown
- Priority: sorted by request assembly match and namespace proximity

Example action/handler:
```csharp
public class LogExceptionAction<TRequest> : IRequestExceptionAction<TRequest, Exception>
{
    public Task Execute(TRequest request, Exception ex, CancellationToken ct)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return Task.CompletedTask;
    }
}

public class TranslateInvalidOpHandler<TRequest, TResult> : IRequestExceptionHandler<TRequest, TResult, InvalidOperationException>
{
    public Task<TResult> Handle(TRequest request, InvalidOperationException ex, CancellationToken ct)
    {
        // Return a default/alternative TResult; domain-specific mapping
        return Task.FromResult(default(TResult)!);
    }
}
```

### Negative Path Demo (Sample)

In the sample project, the Toolbox domain includes:
- `LogExceptionAction<TRequest>`: writes exceptions to stderr
- `TranslateInvalidOpHandler<TRequest>`: maps `InvalidOperationException` to a default `Tool`

Triggering an exception in a handler will execute the action(s) and, if applicable, the handler will produce a safe result instead of throwing.

## Notes

- Stian's Toolbox example demonstrates Dispatcher usage: `AddTool`, `RemoveTool`, `GetTool`, `ListTools`.

Design handlers to return meaningful values for success paths; use exception actions/handlers for cross-cutting error boundaries.

## .NET Targeting

Latest .NET

## Contributors

Thanks to all the contributors and to all the people who gave feedback!

<a href="https://github.com/stianleroux/Dispatch/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=stianleroux/Dispatch" />
</a>

## Copyright

Copyright (c) Stian Le Roux. See LICENSE for details.
