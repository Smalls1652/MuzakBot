using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Extensions;
using MuzakBot.App.Services;
using MuzakBot.Database;
using MuzakBot.Database.Extensions;
using MuzakBot.Database.Models;
using MuzakBot.Hosting.Extensions;
using MuzakBot.Lib.Services;
using MuzakBot.Lib.Services.Extensions;

DateTimeOffset startTime = DateTimeOffset.UtcNow;
Environment.SetEnvironmentVariable("APP_START_TIME", startTime.ToUnixTimeSeconds().ToString());

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSlimHostLifetime();

builder.Services.AddMemoryCache();

builder.Configuration.Sources.Clear();
builder.Configuration
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
    )
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Logging.ClearProviders();

builder.Logging
    .AddSimpleConsole(options =>
    {
        options.IncludeScopes = false;
        options.UseUtcTimestamp = true;
    })
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

builder.Services
    .AddSingleton<IAlbumReleaseReminderMonitorService, AlbumReleaseReminderMonitorService>();

builder.Services
    .AddAppleMusicApiService(options =>
    {
        options.AppleTeamId = builder.Configuration.GetValue<string>("APPLE_TEAM_ID") ?? throw new("APPLE_TEAM_ID is not set.");
        options.AppleAppId = builder.Configuration.GetValue<string>("APPLE_APP_ID") ?? throw new("APPLE_APP_ID is not set.");
        options.AppleAppKeyId = builder.Configuration.GetValue<string>("APPLE_APP_KEY_ID") ?? throw new("APPLE_APP_KEY_ID is not set");
        options.AppleAppKey = builder.Configuration.GetValue<string>("APPLE_APP_KEY") ?? throw new("APPLE_APP_KEY is not set.");

        options.TokenExpiration = builder.Configuration.GetValue<string>("APPLE_TOKEN_EXPIRATION_MINUTES") is not null
            ? TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("APPLE_TOKEN_EXPIRATION_MINUTES"))
            : TimeSpan.FromMinutes(30);
    });

DatabaseConfig databaseConfig = builder.Configuration.GetDatabaseConfig();

builder.Services
    .AddQueueClientService(options =>
    {
        options.ConnectionString = builder.Configuration.GetValue<string>("QUEUE_CONNECTION_STRING") ?? throw new("QUEUE_CONNECTION_STRING is not set.");
        options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new("QUEUE_NAME is not set.");
    });

builder.Services
    .AddMuzakBotDbContextFactory(databaseConfig);

builder.Services
    .AddOpenAiService(options =>
    {
        options.ApiKey = builder.Configuration.GetValue<string>("OPENAI_SECRET_KEY") ?? throw new("OPENAI_SECRET_KEY is not set.");
    })
    .AddGeniusApiService(options =>
    {
        options.AccessToken = builder.Configuration.GetValue<string>("GENIUS_ACCESS_TOKEN") ?? throw new("GENIUS_ACCESS_TOKEN is not set.");
    });

builder.Services.AddDiscordService(options =>
{
    options.AdminGuildId = builder.Configuration.GetValue<ulong>("DISCORD_ADMIN_GUILD");

    options.ClientToken = builder.Configuration.GetValue<string>("DISCORD_CLIENT_TOKEN");

#if DEBUG
    options.TestGuildId = builder.Configuration.GetValue<string>("DISCORD_TEST_GUILD");
#endif
});

using var host = builder.Build();

await host
    .ApplyMuzakBotDbContextMigrations();

await host.RunAsync();
