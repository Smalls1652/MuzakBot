namespace MuzakBot.App.Models.Genius;

/// <summary>
/// Holds data for search results returned by the Genius API.
/// </summary>
public class GeniusSearchResult : IGeniusSearchResult
{
    /// <summary>
    /// Results returned by the Genius API.
    /// </summary>
    [JsonPropertyName("hits")]
    public GeniusSearchResultHitItem[]? Hits { get; set; }
}