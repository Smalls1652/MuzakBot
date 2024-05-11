using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MuzakBot.Hosting.Extensions;

/// <summary>
/// Extension methods for adding the <see cref="SlimHostLifetime"/> to the <see cref="IServiceCollection"/>.
/// </summary>
public static class SlimHostLifetimeExtensions
{
    /// <summary>
    /// Adds the <see cref="SlimHostLifetime"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>A modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddSlimHostLifetime(this IServiceCollection services)
    {
        services.RemoveAll<IHostLifetime>();

        services.AddSingleton<IHostLifetime, SlimHostLifetime>();

        return services;
    }
}
