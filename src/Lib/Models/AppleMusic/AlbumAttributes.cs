namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the attributes data for an Apple Music album.
/// </summary>
public sealed class AlbumAttributes : IAlbumAttributes
{
    /// <summary>
    /// The name of the artist for the album.
    /// </summary>
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    /// <summary>
    /// The album artwork.
    /// </summary>
    [JsonPropertyName("artwork")]
    public Artwork Artwork { get; set; } = null!;

    /// <summary>
    /// The content rating for the album.
    /// </summary>
    [JsonPropertyName("contentRating")]
    public string? ContentRating { get; set; }

    /// <summary>
    /// The copyright text.
    /// </summary>
    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }

    /// <summary>
    /// The names of the genres associated with this album.
    /// </summary>
    [JsonPropertyName("genreNames")]
    public string[] GenreNames { get; set; } = null!;

    /// <summary>
    /// Indicates whether the album is marked as a compilation.
    /// </summary>
    [JsonPropertyName("isCompilation")]
    public bool IsCompilation { get; set; }

    /// <summary>
    /// Indicates whether the album is complete.
    /// </summary>
    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; set; }

    /// <summary>
    /// Indicates whether the album is "Mastered for iTunes".
    /// </summary>
    [JsonPropertyName("isMasteredForItunes")]
    public bool IsMasteredForItunes { get; set; }

    /// <summary>
    /// Indicates whether the album is for a single.
    /// </summary>
    [JsonPropertyName("isSingle")]
    public bool IsSingle { get; set; }

    /// <summary>
    /// The name of the album.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The record label for the album.
    /// </summary>
    [JsonPropertyName("recordLabel")]
    public string? RecordLabel { get; set; }

    /// <summary>
    /// The release date of the album.
    /// </summary>
    [JsonPropertyName("releaseDate")]
    public DateOnly ReleaseDate { get; set; }

    /// <summary>
    /// The number of tracks on the album.
    /// </summary>
    [JsonPropertyName("trackCount")]
    public int TrackCount { get; set; }

    /// <summary>
    /// The Universal Product Code (UPC) for the album.
    /// </summary>
    [JsonPropertyName("upc")]
    public string? Upc { get; set; }

    /// <summary>
    /// The URL for sharing the album on Apple Music.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}
