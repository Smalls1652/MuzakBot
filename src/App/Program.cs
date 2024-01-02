using Discord;
using Discord.WebSocket;
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
using OpenTelemetry.ResourceDetectors.Azure;
using OpenTelemetry.ResourceDetectors.Container;
using MuzakBot.App.Modules;
using OpenTelemetry.Exporter;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();

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

        if (builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            logging.AddAzureMonitorLogExporter(options =>
            {
                options.ConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    });

builder.Services
    .AddMetrics();

builder.Services
    .AddSingleton<CommandMetrics>();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("MuzakBot"))
    .WithMetrics(metrics =>
    {
        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("MuzakBot");

        metrics
            .SetResourceBuilder(resourceBuilder)
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddCommandMetricsInstrumentation();

        metrics.AddOtlpExporter();

        if (builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            metrics.AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    })
    .WithTracing(tracing =>
    {
        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("MuzakBot")
            .AddDetector(new ContainerResourceDetector());
    
        tracing
            .AddSource("MuzakBot.App.Services.DiscordService")
            .AddSource("MuzakBot.App.Modules.ShareMusicCommandModule")
            .SetResourceBuilder(resourceBuilder)
            .AddHttpClientInstrumentation();

        tracing.AddOtlpExporter();

        if (builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            tracing.AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    });

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddHttpClient(
    name: "GenericClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
    }
);

builder.Services.AddHttpClient(
    name: "OdesliApiClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
        httpClient.BaseAddress = new("https://api.song.link/v1-alpha.1/");
    }
);

builder.Services.AddHttpClient(
    name: "MusicBrainzApiClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
        httpClient.BaseAddress = new("https://musicbrainz.org/ws/2/");
    }
);

builder.Services.AddHttpClient(
    name: "ItunesApiClient",
    configureClient: (serviceProvider, httpClient) =>
    {
        httpClient.DefaultRequestHeaders.UserAgent.Add(new("MuzakBot", Assembly.GetExecutingAssembly().GetName().Version!.ToString()));
        httpClient.BaseAddress = new("https://itunes.apple.com/");
    }
);

builder.Services.AddSingleton<DiscordSocketClient>(
    implementationInstance: new(discordSocketConfig)
);

builder.Services.AddSingleton<IDiscordService, DiscordService>();
builder.Services.AddSingleton<IOdesliService, OdesliService>();
builder.Services.AddSingleton<IItunesApiService, ItunesApiService>();
builder.Services.AddSingleton<IMusicBrainzService, MusicBrainzService>();

builder.Logging.AddConsole();

using var host = builder.Build();

var discordService = host.Services.GetRequiredService<IDiscordService>();
await discordService.ConnectAsync();

await host.RunAsync();