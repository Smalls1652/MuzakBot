using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

public static class OpenAiServiceExtensions
{
    public static IServiceCollection AddOpenAiService(this IServiceCollection services, Action<OpenAiServiceOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient(
            name: "OpenAiApiClient",
            configureClient: (serviceProvider, httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
                httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
                httpClient.BaseAddress = new("https://api.openai.com/v1/");
            }
        );

        services.AddSingleton<IOpenAiService, OpenAiService>();

        return services;
    }
}