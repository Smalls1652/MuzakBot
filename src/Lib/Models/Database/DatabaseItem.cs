using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database;

/// <summary>
/// Default properties for a database item.
/// </summary>
public abstract class DatabaseItem
{
    /// <summary>
    /// The unique identifier for the item in the database.
    /// </summary>
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The partition key for item in the database.
    /// </summary>
    [Column("partitionKey")]
    [JsonPropertyName("partitionKey")]
    public string PartitionKey { get; set; } = null!;
}