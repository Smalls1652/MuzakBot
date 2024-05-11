using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add <see cref="AppleMusicApiService"/>.
/// </summary>
public static class AppleMusicApiServiceSetupExtensions
{
    /// <summary>
    /// Add the <see cref="AppleMusicApiService"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="options">An action to configure <see cref="AppleMusicApiService"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddAppleMusicApiService(this IServiceCollection services, Action<AppleMusicApiServiceOptions> options)
    {
        services.Configure(options);

        services.AddHttpClient(
            name: "AppleMusicApiClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
                httpClient.BaseAddress = new("https://api.music.apple.com/");
            }
        );

        services.AddSingleton<IAppleMusicApiService, AppleMusicApiService>();

        return services;
    }
}
