namespace Dispatch;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public sealed class DispatcherOptions
{
    internal readonly List<Assembly> Assemblies = [];
    public void RegisterServicesFromAssembly(Assembly assembly) => Assemblies.Add(assembly);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services, Action<DispatcherOptions>? configure = null)
    {
        services.AddTransient<Dispatcher>();

        var options = new DispatcherOptions();
        configure?.Invoke(options);

        if (options.Assemblies.Count > 0)
        {
            services.Scan(scan => scan
                .FromAssemblies(options.Assemblies)
                .AddClasses(c =>
                    c.Where(t =>
                        t.Name.EndsWith("CommandHandler") ||
                        t.Name.EndsWith("QueryHandler") ||
                        t.Name.EndsWith("NotificationHandler") ||
                        t.Name.EndsWith("ProcessorBehavior")))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }

        return services;
    }
}