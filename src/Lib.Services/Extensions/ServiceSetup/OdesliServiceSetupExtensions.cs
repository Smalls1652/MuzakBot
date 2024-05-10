using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extension methods for adding <see cref="OdesliService"/> to the <see cref="IServiceCollection"/>.
/// </summary>
public static class OdesliServiceSetupExtensions
{
    /// <summary>
    /// Adds the <see cref="OdesliService"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OdesliService"/> to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddOdesliService(this IServiceCollection services)
    {
        services.AddHttpClient(
            name: "OdesliApiClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
                httpClient.BaseAddress = new("https://api.song.link/v1-alpha.1/");
            }
        );

        services.AddSingleton<IOdesliService, OdesliService>();

        return services;
    }
}