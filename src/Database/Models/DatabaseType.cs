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
    /// PostgreSQL database
    /// </summary>
    PostgreSql = 1
}
