namespace MuzakBot.Database.Models;

/// <summary>
/// The type of database to use.
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// SQLite database
    /// </summary>
    Sqlite = 0,

    /// <summary>
    /// Azure Cosmos DB
    /// </summary>
    CosmosDb = 1,

    /// <summary>
    /// PostgreSQL database
    /// </summary>
    PostgreSql = 2
}
