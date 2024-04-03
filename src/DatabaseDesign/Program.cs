using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using MuzakBot.Database.Extensions;
using MuzakBot.Database.Models;

var builder = Host.CreateApplicationBuilder(args);

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

var host = builder.Build();

await host.RunAsync();
