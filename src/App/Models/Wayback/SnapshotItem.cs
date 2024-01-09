namespace MuzakBot.App.Models.Wayback;

public class SnapshotItem
{
    [JsonPropertyName("available")]
    public bool Available { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = null!;
}