namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Holds data for a search result item returned by the Genius API.
/// </summary>
public class GeniusSearchResultItem : IGeniusSearchResultItem
{
    /// <summary>
    /// The API path for the search result item.
    /// </summary>
    [JsonPropertyName("api_path")]
    public string? ApiPath { get; set; }

    /// <summary>
    /// The full title (Includes the song name and artist).
    /// </summary>
    [JsonPropertyName("full_title")]
    public string? FullTitle { get; set; }

    /// <summary>
    /// Names of the artist(s).
    /// </summary>
    [JsonPropertyName("artist_names")]
    public string? ArtistNames { get; set; }

    /// <summary>
    /// The ID for the item.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The title of the item.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The full web page URL for the item.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// The web page URL path for the item.
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>
    /// The state of the lyrics for the item.
    /// </summary>
    [JsonPropertyName("lyrics_state")]
    public string? LyricsState { get; set; }
}
