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
/// Extension methods for the <see cref="LyricsAnalyzerDbContext"/> class.
/// </summary>
public static class LyricsAnalyzerDbContextExtensions
{
    /// <summary>
    /// Adds a <see cref="LyricsAnalyzerDbContext"/> factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseConfig">The database configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid database type is found.</exception>
    public static IServiceCollection AddLyricsAnalyzerDbContextFactory(this IServiceCollection services, DatabaseConfig databaseConfig)
    {
        switch (databaseConfig.DatabaseType)
        {
            case DatabaseType.CosmosDb:
                services.AddLyricsAnalyzerDbContextFactoryForCosmosDb(databaseConfig.CosmosDbConfig!);
                break;

            case DatabaseType.Sqlite:
                services.AddLyricsAnalyzerDbContextFactoryForSqlite(databaseConfig.SqliteDbConfig!);
                break;

            default:
                throw new InvalidOperationException("Invalid database type.");
        }

        return services;
    }

    /// <summary>
    /// Adds a <see cref="LyricsAnalyzerDbContext"/> factory and configures it for Cosmos DB to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="cosmosDbConfig">The Cosmos DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddLyricsAnalyzerDbContextFactoryForCosmosDb(this IServiceCollection services, CosmosDbConfigOptions cosmosDbConfig)
    {
        services.AddDbContextFactory<LyricsAnalyzerDbContext>(options =>
        {
            options.UseCosmos(
                connectionString: cosmosDbConfig.ConnectionString,
                databaseName: "lyrics_analyzer"
            );
        });

        return services;
    }

    /// <summary>
    /// Adds a <see cref="LyricsAnalyzerDbContext"/> factory and configures it for SQLite to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="sqliteDbConfig">The SQLite DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddLyricsAnalyzerDbContextFactoryForSqlite(this IServiceCollection services, SqliteDbConfigOptions sqliteDbConfig)
    {
        services.AddDbContextFactory<LyricsAnalyzerDbContext>(options =>
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
    public static async Task ApplyLyricsAnalyzerDbContextMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<LyricsAnalyzerDbContext>>();

        using var context = dbContextFactory.CreateDbContext();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
            await context.SaveChangesAsync();
        }

        if (context.Database.IsCosmos())
        {
            await context.Database.EnsureCreatedAsync();

            DatabaseUpdate databaseUpdates = await context.DatabaseUpdates
                .WithPartitionKey("applied-updates")
                .FirstOrDefaultAsync()
            ?? new()
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = "applied-updates",
                FirstRun = true,
                MigratedToEfCore = false
            };

            if (!databaseUpdates.MigratedToEfCore)
            {
                using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                    });
                });

                ILogger logger = loggerFactory.CreateLogger("MuzakBot.Database.Upgrade");

                logger.LogWarning("Initial EF Core migration has not been applied yet. Applying migration...");

                await dbContextFactory.EnsureDiscriminatorValuesExistAsync_CosmosDb();

                databaseUpdates.MigratedToEfCore = true;

                if (databaseUpdates.FirstRun)
                {
                    context.DatabaseUpdates.Add(databaseUpdates);
                }
                else
                {
                    context.DatabaseUpdates.Update(databaseUpdates);
                }

                await context.SaveChangesAsync();

                logger.LogInformation("Initial EF Core migration has been applied.");
            }
        }
    }

    /// <summary>
    /// Ensures that discriminator values exist in the Cosmos DB containers.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <returns></returns>
    private static async Task EnsureDiscriminatorValuesExistAsync_CosmosDb(this IDbContextFactory<LyricsAnalyzerDbContext> dbContextFactory)
    {
        Tuple<string, string, string>[] containerUpdates = [
            new("command_configs", "lyricsanalyzer-config", "LyricsAnalyzerConfig"),
            new("prompt_styles", "prompt-style", "LyricsAnalyzerPromptStyle"),
            new("user_rate_limit", "user-item", "LyricsAnalyzerUserRateLimit")
        ];

        foreach (var (containerName, partitionKey, discriminatorValue) in containerUpdates)
        {
            await dbContextFactory.AddDiscriminatorPropertyAsync(
                databaseName: "lyrics_analyzer",
                containerName: containerName,
                partitionKey: partitionKey,
                discriminatorValue: discriminatorValue
            );
        }
    }

    /// <summary>
    /// Adds a discriminator property to documents in a Cosmos DB container.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="containerName">The name of the container.</param>
    /// <param name="partitionKey">The partition key for the items in the container.</param>
    /// <param name="discriminatorValue">The value of the discriminator property.</param>
    /// <returns></returns>
    private static async Task AddDiscriminatorPropertyAsync(this IDbContextFactory<LyricsAnalyzerDbContext> dbContextFactory, string databaseName, string containerName, string partitionKey, string discriminatorValue)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
            });
        });

        ILogger logger = loggerFactory.CreateLogger("MuzakBot.Database.Upgrade");

        using var dbContext = dbContextFactory.CreateDbContext();

        var cosmosClient = dbContext.Database.GetCosmosClient();

        var container = cosmosClient.GetContainer(databaseName, containerName);

        QueryDefinition query = new("SELECT * FROM c");

        using var feedIterator = container.GetItemQueryStreamIterator(
            queryDefinition: query,
            requestOptions: new()
            {
                PartitionKey = new(partitionKey)
            }
        );

        while (feedIterator.HasMoreResults)
        {
            using var response = await feedIterator.ReadNextAsync();

            JsonNode? jsonNode = JsonNode.Parse(
                utf8Json: response.Content
            );

            if (jsonNode is null)
            {
                continue;
            }

            if (jsonNode["Documents"] is null)
            {
                continue;
            }

            foreach (var document in jsonNode["Documents"]!.AsArray())
            {
                if (document is null)
                {
                    continue;
                }

                if (document["Discriminator"] is not null)
                {
                    continue;
                }

                document["Discriminator"] = JsonValue.Create(discriminatorValue);

                using var stream = new MemoryStream();

                await stream.WriteAsync(Encoding.UTF8.GetBytes(document.ToJsonString()));

                await container.ReplaceItemStreamAsync(
                    streamPayload: stream,
                    id: document["id"]!.GetValue<string>(),
                    partitionKey: new(partitionKey)
                );
            }

            logger.LogInformation("Discriminator property added to documents in container '{ContainerName}'.", containerName);
        }
    }
}
