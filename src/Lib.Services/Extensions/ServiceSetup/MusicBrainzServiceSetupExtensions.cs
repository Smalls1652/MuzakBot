using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extension methods for configuring the <see cref="MusicBrainzService"/> in the dependency injection container.
/// </summary>
public static class MusicBrainzServiceSetupExtensions
{
    /// <summary>
    /// Adds the <see cref="MusicBrainzService"/> to the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMusicBrainzService(this IServiceCollection services)
    {
        services.AddHttpClient(
            name: "MusicBrainzApiClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
                httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
                httpClient.BaseAddress = new("https://musicbrainz.org/ws/2/");
            }
        );

        services.AddSingleton<IMusicBrainzService, MusicBrainzService>();

        return services;
    }
}