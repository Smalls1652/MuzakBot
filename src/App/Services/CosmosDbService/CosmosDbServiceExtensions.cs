using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

public static class CosmosDbServiceExtensions
{
    public static IServiceCollection AddCosmosDbService(this IServiceCollection services, Action<CosmosDbServiceOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<ICosmosDbService, CosmosDbService>();

        return services;
    }
}