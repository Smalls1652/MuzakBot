namespace MuzakBot.App.Models.Wayback;

/// <summary>
/// Holds data for a save job's status from the Wayback Machine.
/// </summary>
public class SaveJobStatus : ISaveJobStatus
{
    /// <summary>
    /// The current status of the save job.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    /// <summary>
    /// The timestamp of the save job.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}