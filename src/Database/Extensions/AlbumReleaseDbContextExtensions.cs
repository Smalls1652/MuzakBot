using System.Text;
using System.Text.Json.Nodes;

using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.Database.Models;
using MuzakBot.Lib.Models.Database;

namespace MuzakBot.Database.Extensions;

/// <summary>
/// Extension methods for the <see cref="AlbumReleaseDbContext"/> class.
/// </summary>
public static class AlbumReleaseDbContextExtensions
{
    /// <summary>
    /// Adds a <see cref="AlbumReleaseDbContext"/> factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseConfig">The database configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid database type is found.</exception>
    public static IServiceCollection AddAlbumReleaseDbContextFactory(this IServiceCollection services, DatabaseConfig databaseConfig)
    {
        switch (databaseConfig.DatabaseType)
        {
            case DatabaseType.CosmosDb:
                services.AddAlbumReleaseDbContextFactoryForCosmosDb(databaseConfig.CosmosDbConfig!);
                break;

            case DatabaseType.Sqlite:
                services.AddAlbumReleaseDbContextFactoryForSqlite(databaseConfig.SqliteDbConfig!);
                break;

            default:
                throw new InvalidOperationException("Invalid database type.");
        }

        return services;
    }

    /// <summary>
    /// Adds a <see cref="AlbumReleaseDbContext"/> factory and configures it for Cosmos DB to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="cosmosDbConfig">The Cosmos DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddAlbumReleaseDbContextFactoryForCosmosDb(this IServiceCollection services, CosmosDbConfigOptions cosmosDbConfig)
    {
        services.AddDbContextFactory<AlbumReleaseDbContext>(options =>
        {
            options.UseCosmos(
                connectionString: cosmosDbConfig.ConnectionString,
                databaseName: "album_releases"
            );
        });

        return services;
    }

    /// <summary>
    /// Adds a <see cref="AlbumReleaseDbContext"/> factory and configures it for SQLite to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="sqliteDbConfig">The SQLite DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddAlbumReleaseDbContextFactoryForSqlite(this IServiceCollection services, SqliteDbConfigOptions sqliteDbConfig)
    {
        string dbPath = Path.Join(sqliteDbConfig.DbPath, "album_releases.sqlite");

        services.AddDbContextFactory<AlbumReleaseDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        return services;
    }

    /// <summary>
    /// Applies migrations for <see cref="AlbumReleaseDbContext"/> to the database.
    /// </summary>
    /// <remarks>
    /// This method will not apply migrations if the database is not relational.
    /// </remarks>
    /// <param name="host">The host.</param>
    /// <returns></returns>
    public static async Task ApplyAlbumReleaseDbContextMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AlbumReleaseDbContext>>();

        using var context = dbContextFactory.CreateDbContext();

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
