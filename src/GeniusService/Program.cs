using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.GeniusService.Extensions;
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

builder.Services
    .AddQueueClientService(options =>
    {
        options.ConnectionString = builder.Configuration.GetValue<string>("QUEUE_CONNECTION_STRING") ?? throw new ConfigValueNotFoundException("QUEUE_CONNECTION_STRING");
        options.QueueName = builder.Configuration.GetValue<string>("QUEUE_NAME") ?? throw new ConfigValueNotFoundException("QUEUE_NAME");
    });

builder.Services
    .AddMainService();

var app = builder.Build();

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
    return 1;
}

return 0;
