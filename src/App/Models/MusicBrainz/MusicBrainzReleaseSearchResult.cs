using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.MusicBrainz;

public class MusicBrainzReleaseSearchResult
{
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("releases")]
    public MusicBrainzReleaseItem[]? Releases { get; set; }

    public MusicBrainzReleaseItem[]? GetDistinct()
    {
        return Releases?.DistinctBy(item => item.Title).ToArray();
    }
}