using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.MusicBrainz;

public class MusicBrainzRecordingItem : IMusicBrainzRecordingItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("releases")]
    public MusicBrainzReleaseItem[]? Releases { get; set; }
}