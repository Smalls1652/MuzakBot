using System.Text.Json.Serialization;

namespace MuzakBot.Database.Models;

public class CosmosDbConfigOptions
{
    [JsonPropertyName("COSMOSDB_CONNECTION_STRING")]
    public string ConnectionString { get; set; } = null!;
}
