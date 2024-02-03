namespace MuzakBot.Lib.Models.Database;

/// <summary>
/// Default properties for a database item.
/// </summary>
public abstract class DatabaseItem
{
    /// <summary>
    /// The unique identifier for the item in the database.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The partition key for item in the database.
    /// </summary>
    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = null!;
}