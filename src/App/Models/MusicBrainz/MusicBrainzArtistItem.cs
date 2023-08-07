using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.MusicBrainz;

public class MusicBrainzArtistItem : IMusicBrainzArtistItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("sort-name")]
    public string SortName { get; set; } = null!;

    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}