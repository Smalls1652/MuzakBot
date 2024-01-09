using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddEnvironmentVariables()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(
        path: "appsettings.json",
        optional: true,
        reloadOnChange: true
    )
    .AddJsonFile(
        path: $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    );

builder.Logging.ClearProviders();

builder.Logging
    .AddOpenTelemetryLogging(
        azureAppInsightsConnectionString: builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING")
    );

builder.Services
    .AddOpenTelemetryMetricsAndTracing(
        azureAppInsightsConnectionString: builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING")
    );

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddHttpClient(
    name: "GenericClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
    }
);

builder.Services
    .AddOdesliService()
    .AddItunesApiService()
    .AddMusicBrainzService();

if (builder.Configuration.GetValue<bool>("ENABLE_LYRICS_ANALYZER"))
{
    builder.Services
        .AddOpenAiService(options =>
        {
            options.ApiKey = builder.Configuration.GetValue<string>("OPENAI_SECRET_KEY") ?? throw new("OPENAI_SECRET_KEY is not set.");
        })
        .AddGeniusApiService(options =>
        {
            options.AccessToken = builder.Configuration.GetValue<string>("GENIUS_ACCESS_TOKEN") ?? throw new("GENIUS_ACCESS_TOKEN is not set.");
        });

    builder.Services.AddTransient<DiscordServiceConfig>(serviceProvider => new(builder.Configuration.GetValue<string?>("LYRICS_ANALYZER_SERVERS")));
}

builder.Services.AddDiscordService(options =>
{
    options.ClientToken = builder.Configuration.GetValue<string>("DISCORD_CLIENT_TOKEN");
    options.EnableLyricsAnalyzer = builder.Configuration.GetValue<bool>("ENABLE_LYRICS_ANALYZER");

#if DEBUG
    options.TestGuildId = builder.Configuration.GetValue<string>("DISCORD_TEST_GUILD");
#endif
});

using var host = builder.Build();

await host.RunAsync();
