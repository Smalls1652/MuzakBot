namespace MuzakBot.App.Models.Wayback;

public class ArchivedStatus
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("archived_snapshots")]
    public ArchivedSnapshots? ArchivedSnapshots { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}