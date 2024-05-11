namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the attributes data for an Apple Music artist.
/// </summary>
public sealed class ArtistAttributes : IArtistAttributes
{
    /// <summary>
    /// The artwork for the artist.
    /// </summary>
    [JsonPropertyName("artwork")]
    public Artwork? Artwork { get; set; }

    /// <summary>
    /// The names of the genres associated with this artist.
    /// </summary>
    [JsonPropertyName("genreNames")]
    public string[] GenreNames { get; set; } = null!;

    /// <summary>
    /// The name of the artist.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The URL for sharing the artist in Apple Music.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}
