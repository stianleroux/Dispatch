using Dispatch;
using Dispatch.Sample.Api;
using Dispatch.Sample.Api.Toolbox.V1.Commands;
using Dispatch.Sample.Api.Toolbox.V1.Models;
using Dispatch.Sample.Api.Toolbox.V1.Queries;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        // Point Scalar to the Minimal API OpenAPI JSON route
        options.WithOpenApiRoutePattern("/openapi/v1.json");
        options.WithTitle("Dispatch Sample API");
    });
    // Redirect root to Scalar UI; some environments require the explicit index.html
    app.MapGet("/", () => Microsoft.AspNetCore.Http.Results.Redirect("/scalar/index.html"));
}

app.UseHttpsRedirection();

app.MapPost("/tools", async (AddToolCommand command, Dispatcher dispatcher, CancellationToken ct)
    => await dispatcher.Send<AddToolCommand, Tool>(command, ct));

app.MapDelete("/tools/{id}", async (Guid id, Dispatcher dispatcher, CancellationToken ct)
    => await dispatcher.Send<RemoveToolCommand, bool>(new RemoveToolCommand(id), ct));

app.MapGet("/tools/{id}", async (Guid id, Dispatcher dispatcher, CancellationToken ct)
    => await dispatcher.Query<GetToolQuery, Tool?>(new GetToolQuery(id), ct));

app.MapGet("/tools", async (Dispatcher dispatcher, CancellationToken ct)
    => await dispatcher.Query<ListToolsQuery, IReadOnlyList<Tool>>(new ListToolsQuery(), ct));

app.Run();
