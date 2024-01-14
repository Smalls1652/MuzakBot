namespace MuzakBot.App.Models.CosmosDb;

/// <summary>
/// Response from the Azure CosmosDB API.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CosmosDbResponse<T> : ICosmosDbResponse<T>
{
    /// <inheritdoc />
    [JsonPropertyName("Documents")]
    public T[]? Documents { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("_count")]
    public int Count { get; set; }
}