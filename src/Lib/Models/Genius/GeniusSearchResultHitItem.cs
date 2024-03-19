namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Holds data for a single search result returned by the Genius API.
/// </summary>
public class GeniusSearchResultHitItem : IGeniusSearchResultHitItem
{
    /// <summary>
    /// The index of the item.
    /// </summary>
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    /// <summary>
    /// The type of the item.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// The data for the item.
    /// </summary>
    [JsonPropertyName("result")]
    public GeniusSearchResultItem? Result { get; set; }
}