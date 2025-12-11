namespace Dispatch.Sample.Api;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Dispatch;
using Dispatch.Sample.Api.Toolbox.V1.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Optional: add validators if using FluentValidation
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddDispatcher(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton<IToolRepository, ToolRepository>();

        // Register command and query handlers
        // Handlers are scanned via AddDispatcher options

        services.AddHealthChecks();

        return services;
    }
}