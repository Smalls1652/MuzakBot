using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MuzakBot.Database.Models;

namespace MuzakBot.Database.Extensions;

/// <summary>
/// Extension methods for the <see cref="SongLyricsDbContext"/> class.
/// </summary>
public static class SongLyricsDbContextExtensions
{
    /// <summary>
    /// Adds a <see cref="SongLyricsDbContext"/> factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseConfig">The database configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid database type is found.</exception>
    public static IServiceCollection AddSongLyricsDbContextFactory(this IServiceCollection services, DatabaseConfig databaseConfig)
    {
        switch (databaseConfig.DatabaseType)
        {
            case DatabaseType.CosmosDb:
                services.AddSongLyricsDbContextFactoryForCosmosDb(databaseConfig.CosmosDbConfig!);
                break;

            case DatabaseType.Sqlite:
                services.AddSongLyricsDbContextFactoryForSqlite(databaseConfig.SqliteDbConfig!);
                break;

            default:
                throw new InvalidOperationException("Invalid database type.");
        }

        return services;
    }

    /// <summary>
    /// Adds a <see cref="SongLyricsDbContext"/> factory and configures it for Cosmos DB to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="cosmosDbConfig">The Cosmos DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddSongLyricsDbContextFactoryForCosmosDb(this IServiceCollection services, CosmosDbConfigOptions cosmosDbConfig)
    {
        services.AddDbContextFactory<SongLyricsDbContext>(options =>
        {
            options.UseCosmos(
                connectionString: cosmosDbConfig.ConnectionString,
                databaseName: "lyrics_analyzer"
            );
        });

        return services;
    }

    /// <summary>
    /// Adds a <see cref="SongLyricsDbContext"/> factory and configures it for SQLite to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="sqliteDbConfig">The SQLite DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddSongLyricsDbContextFactoryForSqlite(this IServiceCollection services, SqliteDbConfigOptions sqliteDbConfig)
    {
        services.AddDbContextFactory<SongLyricsDbContext>(options =>
        {
            options.UseSqlite($"Data Source={sqliteDbConfig.DbPath}");
        });

        return services;
    }

    /// <summary>
    /// Applies migrations for <see cref="SongLyricsDbContext"/> to the database.
    /// </summary>
    /// <remarks>
    /// This method will not apply migrations if the database is not relational.
    /// </remarks>
    /// <param name="host">The host.</param>
    /// <returns></returns>
    public static async Task ApplySongLyricsDbContextMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SongLyricsDbContext>>();

        using var context = db.CreateDbContext();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
            await context.SaveChangesAsync();
        }

        if (context.Database.IsCosmos())
        {
            await context.Database.EnsureCreatedAsync();
        }
    }
}
