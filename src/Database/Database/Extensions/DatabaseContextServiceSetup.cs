using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MuzakBot.Database.Models;

namespace MuzakBot.Database.Extensions;

public static class DatabaseContextServiceSetup
{
    public static IServiceCollection AddMuzakbotDatabaseContexts(this IServiceCollection services, DatabaseConfigOptions options)
    {
        switch (options.DatabaseType)
        {
            case DatabaseType.CosmosDb:
                services.AddCosmosDbContexts(options.CosmosDbConfig!);
                break;

            case DatabaseType.Sqlite:
                services.AddSqliteContexts(options.SqliteConfig!);
                break;

            default:
                throw new InvalidOperationException("Invalid database type.");
        }

        return services;
    }

    private static IServiceCollection AddSqliteContexts(this IServiceCollection services, SqliteConfigOptions sqliteOptions)
    {
        sqliteOptions.EnsureDatabaseDirectoryExists();

        Console.WriteLine($"Adding LyricsAnalyzerContext with SQLite database: {Path.Combine(sqliteOptions.DbDirPath, "lyrics-analyzer.sqlite")}");
        services.AddDbContextFactory<LyricsAnalyzerContext>(options =>
        {
            options.UseSqlite($"Data Source={Path.Combine(sqliteOptions.DbDirPath, "lyrics-analyzer.sqlite")}");
        });

        return services;
    }

    private static IServiceCollection AddCosmosDbContexts(this IServiceCollection services, CosmosDbConfigOptions cosmosDbOptions)
    {
        services.AddDbContext<LyricsAnalyzerContext>(options =>
        {
            options.UseCosmos(
                connectionString: cosmosDbOptions.ConnectionString,
                databaseName: "lyrics-analyzer"
            );
        });

        return services;
    }
}
