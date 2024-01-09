namespace MuzakBot.App.Models.Wayback;

public class SaveJobStatus : ISaveJobStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}