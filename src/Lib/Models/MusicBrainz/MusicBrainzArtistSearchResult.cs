using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.MusicBrainz;

/// <summary>
/// Represents an artists search result retrieved from MusicBrainz.
/// </summary>
public class MusicBrainzArtistSearchResult
{
    /// <summary>
    /// Gets or sets the date and time when the search result was created.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the total number of artists matching the search criteria.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the offset of the first artist in the search result.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the array of artists matching the search criteria.
    /// </summary>
    [JsonPropertyName("artists")]
    public MusicBrainzArtistItem[]? Artists { get; set; }
}