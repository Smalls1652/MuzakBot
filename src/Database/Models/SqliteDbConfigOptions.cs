using System.Text.Json.Serialization;

namespace MuzakBot.Database.Models;

/// <summary>
/// Configuration options for SQLite.
/// </summary>
public sealed class SqliteDbConfigOptions
{
    /// <summary>
    /// The path to the SQLite database file.
    /// </summary>
    [JsonPropertyName("SQLITE_DB_PATH")]
    public string DbPath { get; set; } = null!;
}