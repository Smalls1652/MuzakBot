namespace MuzakBot.Lib.Models.Wayback;

/// <summary>
/// Represents the archive status of a URL from the Wayback Machine.
/// </summary>
public class ArchivedStatus
{
    /// <summary>
    /// The source URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    /// <summary>
    /// Snapshots of the archived page.
    /// </summary>
    [JsonPropertyName("archived_snapshots")]
    public ArchivedSnapshots? ArchivedSnapshots { get; set; }

    /// <summary>
    /// The timestamp of the archived page.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}
