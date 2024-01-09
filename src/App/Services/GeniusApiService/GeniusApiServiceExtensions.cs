using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

public static class GeniusApiServiceExtensions
{
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