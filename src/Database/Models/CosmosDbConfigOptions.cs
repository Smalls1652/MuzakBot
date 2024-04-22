using System.Text.Json.Serialization;

namespace MuzakBot.Database.Models;

/// <summary>
/// Configuration options for Cosmos DB.
/// </summary>
public sealed class CosmosDbConfigOptions
{
    /// <summary>
    /// The connection string for Cosmos DB.
    /// </summary>
    [JsonPropertyName("COSMOSDB_CONNECTIONSTRING")]
    public string ConnectionString { get; set; } = null!;
}
