using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

public static class OdesliServiceExtensions
{
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