namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the results data for an Apple Music search request.
/// </summary>
/// <typeparam name="T">The type of data returned by the request.</typeparam>
public sealed class SearchResponse<T>
{
    /// <summary>
    /// The results of the search request.
    /// </summary>
    [JsonPropertyName("results")]
    public SearchResponseResults<T> Results { get; set; } = null!;
}
