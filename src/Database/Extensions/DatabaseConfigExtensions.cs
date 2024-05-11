using Microsoft.Extensions.Configuration;

using MuzakBot.Database.Models;
using MuzakBot.Lib;

namespace MuzakBot.Database.Extensions;

/// <summary>
/// Extension methods for getting database configurations.
/// </summary>
public static class DatabaseConfigExtensions
{
    /// <summary>
    /// Gets the database configuration from <see cref="ConfigurationManager"/>.
    /// </summary>
    /// <param name="configuration">The configuration manager.</param>
    /// <returns>The database configuration.</returns>
    /// <exception cref="ConfigValueNotFoundException">Thrown when a required configuration value is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an invalid database type is found.</exception>
    public static DatabaseConfig GetDatabaseConfig(this ConfigurationManager configuration)
    {
        string databaseTypeString = configuration.GetValue<string>("DATABASE_TYPE") ?? throw new ConfigValueNotFoundException("DATABASE_TYPE");

        DatabaseType databaseType = Enum.Parse<DatabaseType>(databaseTypeString, true);

        DatabaseConfig databaseConfig = databaseType switch
        {
            DatabaseType.CosmosDb => new()
            {
                DatabaseType = databaseType,
                CosmosDbConfig = new()
                {
                    ConnectionString = configuration.GetValue<string>("COSMOSDB_CONNECTIONSTRING") ?? throw new ConfigValueNotFoundException("COSMOSDB_CONNECTIONSTRING")
                }
            },

            DatabaseType.Sqlite => new()
            {
                DatabaseType = databaseType,
                SqliteDbConfig = new()
                {
                    DbPath = Path.GetFullPath(configuration.GetValue<string>("SQLITE_DB_PATH") ?? throw new ConfigValueNotFoundException("SQLITE_DB_PATH"))
                }
            },

            _ => throw new InvalidOperationException("Invalid database type.")
        };

        return databaseConfig;
    }
}
