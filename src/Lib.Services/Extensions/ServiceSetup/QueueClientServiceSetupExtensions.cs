using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extension methods for setting up <see cref="IQueueClientService"/> services.
/// </summary>
public static class QueueClientServiceExtensions
{
    /// <summary>
    /// Adds an <see cref="IQueueClientService"/> service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="options">A delegate that is used to configure the <see cref="QueueClientServiceOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQueueClientService(this IServiceCollection services, Action<QueueClientServiceOptions> options)
    {
        services.Configure(options);

        services.AddTransient<IQueueClientService, QueueClientService>();

        return services;
    }

    /// <summary>
    /// Adds a keyed <see cref="IQueueClientService"/> service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="keyName">The key name for the service.</param>
    /// <param name="options">A delegate that is used to configure the <see cref="QueueClientServiceOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddKeyedQueueClientService(this IServiceCollection services, string keyName, Action<QueueClientServiceOptions> options)
    {
        services.Configure(
            name: keyName,
            configureOptions: options
        );

        services.AddKeyedTransient<IQueueClientService, QueueClientService>(keyName);

        return services;
    }
}