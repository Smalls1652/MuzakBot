using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.MusicBrainz;

public class MusicBrainzReleaseItem : IMusicBrainzReleaseItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }
}