namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds data about the artwork for an Apple Music item.
/// </summary>
public sealed class Artwork : IArtwork
{
    /// <summary>
    /// The average background color of the image.
    /// </summary>
    [JsonPropertyName("bgColor")]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// The maximum height of the image.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// The maximum width of the image.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// The primary text color used if the background color gets displayed.
    /// </summary>
    [JsonPropertyName("textColor1")]
    public string? TextColor1 { get; set; }

    /// <summary>
    /// The secondary text color used if the background color gets displayed.
    /// </summary>
    [JsonPropertyName("textColor2")]
    public string? TextColor2 { get; set; }

    /// <summary>
    /// The tertiary text color used if the background color gets displayed.
    /// </summary>
    [JsonPropertyName("textColor3")]
    public string? TextColor3 { get; set; }

    /// <summary>
    /// The final post-tertiary text color used if the background color gets displayed.
    /// </summary>
    [JsonPropertyName("textColor4")]
    public string? TextColor4 { get; set; }

    /// <summary>
    /// The URL to request the image asset.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
}