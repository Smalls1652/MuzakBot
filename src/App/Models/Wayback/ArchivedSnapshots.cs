namespace MuzakBot.App.Models.Wayback;
public class ArchivedSnapshots
{
    [JsonPropertyName("closest")]
    public SnapshotItem? Closest { get; set; }
}