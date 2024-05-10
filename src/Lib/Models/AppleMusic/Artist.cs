namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds data for an Apple Music artist.
/// </summary>
public sealed class Artist : IArtist
{
    /// <summary>
    /// The identifier for the artist.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The type of the item.
    /// </summary>
    /// <remarks>
    /// This value will always be "artists".
    /// </remarks>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// The relative location for the artist resource.
    /// </summary>
    [JsonPropertyName("href")]
    public string Href { get; set; } = null!;

    /// <summary>
    /// The attributes for the artist.
    /// </summary>
    [JsonPropertyName("attributes")]
    public ArtistAttributes? Attributes { get; set; }
}