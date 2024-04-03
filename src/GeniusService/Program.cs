using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.Database.Extensions;
using MuzakBot.Database.Models;
using MuzakBot.GeniusService.Extensions;
using MuzakBot.GeniusService.Services;
using MuzakBot.GeniusService.TaskQueues;
using MuzakBot.Hosting.Extensions;
using MuzakBot.Lib;
using MuzakBot.Lib.Services.Extensions;

using ILoggerFactory appLoggerFactory = LoggerFactory.Create(configure =>
{
    configure.AddSimpleConsole();
});

ILogger appLogger = appLoggerFactory.CreateLogger("MuzakBot.GeniusService");

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSlimHostLifetime();

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
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

DatabaseConfig databaseConfig = builder.Configuration.GetDatabaseConfig();

builder.Services
    .AddSongLyricsDbContextFactory(databaseConfig);

builder.Services
    .AddSingleton<IBackgroundTaskQueue>(_ =>
    {
        return new DefaultBackgroundTaskQueue(100);
    });

builder.Services
    .AddGeniusApiService(options =>
    {
        options.AccessToken = builder.Configuration.GetValue<string>("GENIUS_ACCESS_TOKEN") ?? throw new ConfigValueNotFoundException("GENIUS_ACCESS_TOKEN");
    });

builder.Services
    .AddQueueClientService(options =>
    {
        options.ConnectionString = builder.Configuration.GetValue<string>("QUEUE_CONNECTION_STRING") ?? throw new ConfigValueNotFoundException("QUEUE_CONNECTION_STRING");
        options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new ConfigValueNotFoundException("QUEUE_NAME");
    });

builder.Services
    .AddSingleton<IAzureQueueMonitorService, AzureQueueMonitorService>();

builder.Services
    .AddMainService();

var app = builder.Build();

await app.ApplySongLyricsDbContextMigrations();

try
{
    await app.RunAsync();
}
catch (ConfigValueNotFoundException ex)
{
    appLogger.LogError("{Message}", ex.Message);

    return 1;
}
catch (Exception)
{
    throw;
}

return 0;
