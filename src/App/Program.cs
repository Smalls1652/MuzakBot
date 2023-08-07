using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using System.Reflection;
using MuzakBot.App.Services;

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