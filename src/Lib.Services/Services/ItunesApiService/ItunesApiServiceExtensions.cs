using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Extension methods for configuring the <see cref="ItunesApiService"/> in the dependency injection container.
/// </summary>
public static class ItunesApiServiceExtensions
{
    /// <summary>
    /// Adds the <see cref="ItunesApiService"/> to the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddItunesApiService(this IServiceCollection services)
    {
        services.AddHttpClient(
            name: "ItunesApiClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
                httpClient.BaseAddress = new("https://itunes.apple.com/");
            }
        );

        services.AddSingleton<IItunesApiService, ItunesApiService>();

        return services;
    }
}