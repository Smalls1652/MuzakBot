using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.MusicBrainz;

public class MusicBrainzRecordingSearchResult
{
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("recordings")]
    public MusicBrainzRecordingItem[]? Recordings { get; set; }
}