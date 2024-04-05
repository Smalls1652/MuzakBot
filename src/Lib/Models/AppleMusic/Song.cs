namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds data about a song from Apple Music.
/// </summary>
public sealed class Song : ISong
{
    /// <summary>
    /// The identifier for the song.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The type of the resource.
    /// </summary>
    /// <remarks>
    /// This will always be "songs".
    /// </remarks>
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    /// <summary>
    /// The relative location for the song resource.
    /// </summary>
    [JsonPropertyName("href")]
    public string Href { get; set; } = null!;

    /// <summary>
    /// The attributes for the song.
    /// </summary>
    [JsonPropertyName("attributes")]
    public SongAttributes? Attributes { get; set; }
}
