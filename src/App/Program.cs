using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using MuzakBot.App.Services;
using MuzakBot.App.Extensions;

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

builder.Services.AddDiscordService(options =>
{
    options.ClientToken = builder.Configuration.GetValue<string>("DISCORD_CLIENT_TOKEN");

#if DEBUG
    options.TestGuildId = builder.Configuration.GetValue<string>("DISCORD_TEST_GUILD");
#endif
});

using var host = builder.Build();

await host.RunAsync();
