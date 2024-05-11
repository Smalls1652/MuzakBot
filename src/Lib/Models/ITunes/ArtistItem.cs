using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.Itunes;

/// <summary>
/// Represents an artist item retrieved from iTunes.
/// </summary>
public class ArtistItem : IArtistItem
{
    /// <summary>
    /// Gets or sets the wrapper type of the artist item.
    /// </summary>
    [JsonPropertyName("wrapperType")]
    public string WrapperType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the artist type.
    /// </summary>
    [JsonPropertyName("artistType")]
    public string ArtistType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the artist.
    /// </summary>
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URL of the artist's link.
    /// </summary>
    [JsonPropertyName("artistLinkUrl")]
    public string ArtistLinkUrl { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the artist.
    /// </summary>
    [JsonPropertyName("artistId")]
    public long ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the AMG artist ID.
    /// </summary>
    [JsonPropertyName("amgArtistId")]
    public long? AmgArtistId { get; set; }

    /// <summary>
    /// Gets or sets the primary genre name of the artist.
    /// </summary>
    [JsonPropertyName("primaryGenreName")]
    public string PrimaryGenreName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the primary genre ID of the artist.
    /// </summary>
    [JsonPropertyName("primaryGenreId")]
    public long PrimaryGenreId { get; set; }
}
