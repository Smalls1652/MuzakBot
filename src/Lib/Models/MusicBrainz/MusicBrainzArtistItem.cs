using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.MusicBrainz;

/// <summary>
/// Represents an artist item retrieved from MusicBrainz.
/// </summary>
public class MusicBrainzArtistItem : IMusicBrainzArtistItem
{
    /// <summary>
    /// Gets or sets the ID of the artist.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the artist.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sort name of the artist.
    /// </summary>
    [JsonPropertyName("sort-name")]
    public string SortName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the disambiguation of the artist.
    /// </summary>
    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }

    /// <summary>
    /// Gets or sets the country of the artist.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the type of the artist.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
