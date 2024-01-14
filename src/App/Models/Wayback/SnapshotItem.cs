namespace MuzakBot.App.Models.Wayback;

/// <summary>
/// Represents a snapshot item from the Wayback Machine.
/// </summary>
public class SnapshotItem
{
    /// <summary>
    /// Whether or not the snapshot is available.
    /// </summary>
    [JsonPropertyName("available")]
    public bool Available { get; set; }

    /// <summary>
    /// The URL of the snapshot.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    /// <summary>
    /// The timestamp of the snapshot.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = null!;
}