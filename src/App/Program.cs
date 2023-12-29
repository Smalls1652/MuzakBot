using Discord;
using Discord.WebSocket;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using System.Reflection;
using MuzakBot.App.Services;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using MuzakBot.App.Metrics;
using OpenTelemetry.Trace;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddMemoryCache();

GatewayIntents gatewayIntents = GatewayIntents.AllUnprivileged - GatewayIntents.GuildInvites - GatewayIntents.GuildScheduledEvents;

#if DEBUG
DiscordSocketConfig discordSocketConfig = new()
{
    GatewayIntents = gatewayIntents,
    UseInteractionSnowflakeDate = false
};
#else
DiscordSocketConfig discordSocketConfig = new()
{
    GatewayIntents = gatewayIntents
};
#endif

hostBuilder.Configuration.Sources.Clear();
hostBuilder.Configuration
    .AddEnvironmentVariables()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(
        path: "appsettings.json",
        optional: true,
        reloadOnChange: true
    )
    .AddJsonFile(
        path: $"appsettings.{hostBuilder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    );

hostBuilder.Logging.ClearProviders();

hostBuilder.Logging
    .AddOpenTelemetry(logging =>
    {
        logging.IncludeScopes = true;
        logging.IncludeFormattedMessage = true;

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("MuzakBot");

        logging
            .SetResourceBuilder(resourceBuilder)
            .AddConsoleExporter()
            .AddOtlpExporter();

        if (hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            logging.AddAzureMonitorLogExporter(options =>
            {
                options.ConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    });

hostBuilder.Services
    .AddMetrics();

hostBuilder.Services
    .AddSingleton<CommandMetrics>();

hostBuilder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("MuzakBot"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddCommandMetricsInstrumentation();

        metrics.AddOtlpExporter();

        if (hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            metrics.AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddHttpClientInstrumentation();

        tracing.AddOtlpExporter();

        if (hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            tracing.AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    });

Console.WriteLine($"Environment: {hostBuilder.Environment.EnvironmentName}");

hostBuilder.Services.AddHttpClient(
    name: "GenericClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
    }
);

hostBuilder.Services.AddHttpClient(
    name: "OdesliApiClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
        httpClient.BaseAddress = new("https://api.song.link/v1-alpha.1/");
    }
);

hostBuilder.Services.AddHttpClient(
    name: "MusicBrainzApiClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
        httpClient.BaseAddress = new("https://musicbrainz.org/ws/2/");
    }
);

hostBuilder.Services.AddHttpClient(
    name: "ItunesApiClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
        httpClient.BaseAddress = new("https://itunes.apple.com/");
    }
);

hostBuilder.Services.AddSingleton<DiscordSocketClient>(
    implementationInstance: new(discordSocketConfig)
);

hostBuilder.Services.AddSingleton<IDiscordService, DiscordService>();
hostBuilder.Services.AddSingleton<IOdesliService, OdesliService>();
hostBuilder.Services.AddSingleton<IItunesApiService, ItunesApiService>();
hostBuilder.Services.AddSingleton<IMusicBrainzService, MusicBrainzService>();

hostBuilder.Logging.AddConsole();

using var host = hostBuilder.Build();

var discordService = host.Services.GetRequiredService<IDiscordService>();
await discordService.ConnectAsync();

await host.RunAsync();