namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Holds data returned by the Genius API.
/// </summary>
/// <typeparam name="T">The type of result returned by the API.</typeparam>
public class GeniusApiResponse<T> : IGeniusApiResponse<T>
{
    /// <summary>
    /// Metadata for the API response.
    /// </summary>
    [JsonPropertyName("meta")]
    public GeniusMeta Meta { get; set; } = new();

    /// <summary>
    /// The response from the API.
    /// </summary>
    [JsonPropertyName("response")]
    public T? Response { get; set; }
}
