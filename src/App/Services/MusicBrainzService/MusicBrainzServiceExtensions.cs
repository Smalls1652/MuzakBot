using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

public static class MusicBrainzServiceExtensions
{
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