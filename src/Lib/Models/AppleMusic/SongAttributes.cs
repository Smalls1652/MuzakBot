namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the attributes data for a song from Apple Music.
/// </summary>
public sealed class SongAttributes : ISongAttributes
{
    /// <summary>
    /// The name of the album that the song is from.
    /// </summary>
    [JsonPropertyName("albumName")]
    public string AlbumName { get; set; } = null!;

    /// <summary>
    /// The name of the artist that made the song.
    /// </summary>
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    /// <summary>
    /// The URL for the artist that made the song.
    /// </summary>
    [JsonPropertyName("artistUrl")]
    public string ArtistUrl { get; set; } = null!;

    /// <summary>
    /// The artwork for the song.
    /// </summary>
    [JsonPropertyName("artwork")]
    public Artwork Artwork { get; set; } = null!;

    /// <summary>
    /// The name of the composer for the song.
    /// </summary>
    [JsonPropertyName("composerName")]
    public string? ComposerName { get; set; }

    /// <summary>
    /// The content rating for the song.
    /// </summary>
    [JsonPropertyName("contentRating")]
    public string? ContentRating { get; set; }

    /// <summary>
    /// The disc number for the song.
    /// </summary>
    [JsonPropertyName("discNumber")]
    public int DiscNumber { get; set; }

    /// <summary>
    /// The duration of the song in milliseconds.
    /// </summary>
    [JsonPropertyName("durationInMillis")]
    public int DurationInMillis { get; set; }

    /// <summary>
    /// The genre names the song is associated with.
    /// </summary>
    [JsonPropertyName("genreNames")]
    public string[] GenreNames { get; set; } = null!;

    /// <summary>
    /// Indicates whether the song has lyrics.
    /// </summary>
    [JsonPropertyName("hasLyrics")]
    public bool HasLyrics { get; set; }

    /// <summary>
    /// The International Standard Recording Code (ISRC) for the song.
    /// </summary>
    [JsonPropertyName("isrc")]
    public string? Isrc { get; set; }

    /// <summary>
    /// The name of the song.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The release date of the song.
    /// </summary>
    [JsonPropertyName("releaseDate")]
    public DateOnly ReleaseDate { get; set; }

    /// <summary>
    /// The track number for the song.
    /// </summary>
    [JsonPropertyName("trackNumber")]
    public int TrackNumber { get; set; }

    /// <summary>
    /// The URL for sharing the song in Apple Music.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}
