using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extension methods for adding the Genius API service to the service collection.
/// </summary>
public static class GeniusApiServiceSetupExtensions
{
    /// <summary>
    /// Adds the <see cref="GeniusApiService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This configures the service with:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// An <see cref="HttpClient"/> named <c>GeniusClient</c> with a base address of <c>https://genius.com/</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// An <see cref="HttpClient"/> named <c>GeniusApiClient</c> with a base address of <c>https://api.genius.com/</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// An <see cref="IGeniusApiService"/> implementation of <see cref="GeniusApiService"/>.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">An action to configure the <see cref="GeniusApiService"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddGeniusApiService(this IServiceCollection services, Action<GeniusApiServiceOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient(
            name: "GeniusClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.BaseAddress = new("https://genius.com/");
            }
        );

        services.AddHttpClient(
            name: "InternetArchiveClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.BaseAddress = new("https://archive.org/");
            }
        );

        services.AddHttpClient(
            name: "GeniusApiClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
                httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
                httpClient.BaseAddress = new("https://api.genius.com/");
            }
        );

        services.AddSingleton<IGeniusApiService, GeniusApiService>();

        return services;
    }
}