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

    /// <summary>
    /// Get the album artwork URL with a resolution of 512x512.
    /// </summary>
    /// <returns>A string representing the URL to the album artwork.</returns>
    public string GetAlbumArtworkUrl() => GetAlbumArtworkUrlByResolution(512, 512);

    /// <summary>
    /// Get the album artwork URL with a specific resolution.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height</param>
    /// <returns>A string reprensenting the URL to the album artwork.</returns>
    /// <exception cref="ArgumentException">Thrown when the requested resolution is larger than the original image.</exception>
    public string GetAlbumArtworkUrlByResolution(int width, int height)
    {
        if (width > Width || height > Height)
        {
            throw new ArgumentException("The requested resolution is larger than the original image.");
        }

        return Url
            .Replace(
                oldValue: "{w}",
                newValue: width.ToString()
            )
            .Replace(
                oldValue: "{h}",
                newValue: height.ToString()
            );
    }

    /// <summary>
    /// Get a stream of the album artwork.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>A stream of the album artwork.</returns>
    public async Task<Stream> GetAlbumArtworkStreamAsync(int width, int height)
    {
        using HttpClient client = new();

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: GetAlbumArtworkUrlByResolution(width, height)
        );

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        MemoryStream memoryStream = new();

        await responseMessage.Content.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        return memoryStream;
    }
}