using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.MusicBrainz;

/// <summary>
/// Represents a recording item from MusicBrainz.
/// </summary>
public class MusicBrainzRecordingItem : IMusicBrainzRecordingItem
{
    /// <summary>
    /// Gets or sets the ID of the recording.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the recording.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the releases associated with the recording.
    /// </summary>
    [JsonPropertyName("releases")]
    public MusicBrainzReleaseItem[]? Releases { get; set; }
}
