namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the results data for an Apple Music search request.
/// </summary>
public sealed class SearchResponse
{
    /// <summary>
    /// The results of the search request.
    /// </summary>
    [JsonPropertyName("results")]
    public SearchResponseResults Results { get; set; } = null!;
}
