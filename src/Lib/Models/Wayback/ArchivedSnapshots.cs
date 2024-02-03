namespace MuzakBot.Lib.Models.Wayback;

/// <summary>
/// Represents a snapshot item from the Wayback Machine.
/// </summary>
public class ArchivedSnapshots
{
    /// <summary>
    /// The latest snapshot.
    /// </summary>
    [JsonPropertyName("closest")]
    public SnapshotItem? Closest { get; set; }
}