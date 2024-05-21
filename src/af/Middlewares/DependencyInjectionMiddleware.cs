using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace af.Middlewares;

internal static class DependencyInjectionMiddleware
{
    public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<ServiceCollection> configureServices)
    {
        return UseDependencyInjection(builder, (_, services) => configureServices(services));
    }

    public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<InvocationContext, ServiceCollection> configureServices)
    {
        return builder.AddMiddleware(async (context, next) =>
        {
            var services = new ServiceCollection();
            configureServices(context, services);
            var uniqueServiceTypes = new HashSet<Type>(services.Select(x => x.ServiceType));

            services.TryAddSingleton(context.Console);

            await using var serviceProvider = services.BuildServiceProvider();

            context.BindingContext.AddService<IServiceProvider>(_ => serviceProvider);

            foreach (var serviceType in uniqueServiceTypes)
            {
                context.BindingContext.AddService(serviceType, _ => serviceProvider.GetRequiredService(serviceType));

                var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                context.BindingContext.AddService(enumerableServiceType, _ => serviceProvider.GetServices(serviceType));
            }

            await next(context);
        });
    }
}