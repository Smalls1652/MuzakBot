using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/> to add the <see cref="CosmosDbService"/>.
/// </summary>
public static class CosmosDbServiceSetupExtensions
{
    /// <summary>
    /// Adds the <see cref="CosmosDbService"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The <see cref="CosmosDbServiceOptions"/> to configure the service.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCosmosDbService(this IServiceCollection services, Action<CosmosDbServiceOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<ICosmosDbService, CosmosDbService>();

        return services;
    }
}