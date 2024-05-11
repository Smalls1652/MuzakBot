using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.Lib.Services.Extensions;

/// <summary>
/// Extension methods for adding the OpenAI service to the service collection.
/// </summary>
public static class OpenAiServiceSetupExtensions
{
    /// <summary>
    /// Adds the <see cref="OpenAiService"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">An action to configure the <see cref="OpenAiService"/>.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
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
