using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

public static class IItunesApiServiceExtensions
{
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