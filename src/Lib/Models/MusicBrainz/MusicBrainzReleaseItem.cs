using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.MusicBrainz;

/// <summary>
/// Represents a release item in the MusicBrainz database.
/// </summary>
public class MusicBrainzReleaseItem : IMusicBrainzReleaseItem
{
    /// <summary>
    /// Gets or sets the ID of the release item.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title of the release item.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the barcode of the release item.
    /// </summary>
    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    /// <summary>
    /// Gets or sets the date of the release item.
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }
}