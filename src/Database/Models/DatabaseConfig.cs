namespace MuzakBot.Database.Models;

/// <summary>
/// Configuration options for the app's database.
/// </summary>
public sealed class DatabaseConfig
{
    /// <summary>
    /// The type of database to use.
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// Configuration options for Cosmos DB.
    /// </summary>
    public CosmosDbConfigOptions? CosmosDbConfig { get; set; }

    /// <summary>
    /// Configuration options for SQLite.
    /// </summary>
    public SqliteDbConfigOptions? SqliteDbConfig { get; set; }
}