using System.Text.Json.Serialization;

namespace MuzakBot.Database.Models;

/// <summary>
/// Configuration options for PostgreSQL.
/// </summary>
public sealed class PostgresConfigOptions
{
    /// <summary>
    /// The connection string for PostgreSQL.
    /// </summary>
    [JsonPropertyName("POSTGRES_CONNECTIONSTRING")]
    public string ConnectionString { get; set; } = null!;
}
