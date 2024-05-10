namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the data for an Apple Music album.
/// </summary>
public sealed class Album : IAlbum
{
    /// <summary>
    /// The identifier for the album.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The type of the item.
    /// </summary>
    /// <remarks>
    /// This value will always be "albums".
    /// </remarks>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// The relative location for the album resource.
    /// </summary>
    [JsonPropertyName("href")]
    public string Href { get; set; } = null!;

    /// <summary>
    /// The attributes for the album.
    /// </summary>
    [JsonPropertyName("attributes")]
    public AlbumAttributes? Attributes { get; set; }
}