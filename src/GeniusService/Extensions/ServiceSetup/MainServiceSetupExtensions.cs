using Microsoft.Extensions.DependencyInjection;

using MuzakBot.GeniusService.Services;

namespace MuzakBot.GeniusService.Extensions;

/// <summary>
/// Extension methods for setting up internal services into an <see cref="IServiceCollection"/>.
/// </summary>
internal static partial class ServiceSetupExtensions
{
    /// <summary>
    /// Adds the main service to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMainService(this IServiceCollection services) => AddMainService(services, _ => { });
    
    /// <summary>
    /// Adds the main service to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">A delegate to configure the provided <see cref="MainServiceOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMainService(this IServiceCollection services, Action<MainServiceOptions> configure)
    {
        services.Configure(configure);

        services.AddHostedService<MainService>();;

        return services;
    }
}
