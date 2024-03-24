using System.Text.Json.Serialization;

namespace MuzakBot.Database.Models;

public class DatabaseConfigOptions
{
    [JsonPropertyName("DATABASE_TYPE")]
    [JsonConverter(typeof(JsonStringEnumConverter<DatabaseType>))]
    public DatabaseType DatabaseType { get; set; } = DatabaseType.Sqlite;

    [JsonPropertyName("SQLITE_CONFIG")]
    public SqliteConfigOptions? SqliteConfig { get; set; }

    [JsonPropertyName("COSMOSDB_CONFIG")]
    public CosmosDbConfigOptions? CosmosDbConfig { get; set; }
}
