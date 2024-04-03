using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Extensions;
using MuzakBot.App.Services;
using MuzakBot.Database.Extensions;
using MuzakBot.Database.Models;
using MuzakBot.Lib.Services;
using MuzakBot.Lib.Services.Extensions;

var builder = Host.CreateApplicationBuilder(args);

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

DatabaseConfig databaseConfig = builder.Configuration.GetDatabaseConfig();

builder.Services
    .AddSongLyricsDbContextFactory(databaseConfig);

builder.Services
    .AddCosmosDbService(options =>
    {
        options.ConnectionString = builder.Configuration.GetValue<string>("COSMOSDB_CONNECTION_STRING") ?? throw new("COSMOSDB_CONNECTION_STRING is not set.");
        options.DatabaseName = builder.Configuration.GetValue<string>("COSMOSDB_DATABASE_NAME") ?? throw new("COSMOSDB_DATABASE_NAME is not set.");
    });

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
    .ApplySongLyricsDbContextMigrations();

// Initialize the database and containers for the Cosmos DB service
// before running the host.
await host.Services.GetRequiredService<ICosmosDbService>().InitializeDatabaseAsync();

await host.RunAsync();
