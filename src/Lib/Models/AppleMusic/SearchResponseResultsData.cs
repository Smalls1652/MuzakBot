namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Holds the results data for an Apple Music search request.
/// </summary>
/// <typeparam name="T">The type of data returned by the request.</typeparam>
public class SearchResponseResultsData<T>
{
    /// <summary>
    /// Data returned by the request.
    /// </summary>
    [JsonPropertyName("data")]
    public T[] Data { get; set; } = null!;

    /// <summary>
    /// The relative location to fetch the search result.
    /// </summary>
    [JsonPropertyName("href")]
    public string? Href { get; set; }

    /// <summary>
    /// A relative cursor to fetch the next paginated collection of resources in the result, if more exist.
    /// </summary>
    [JsonPropertyName("next")]
    public string? Next { get; set; }
}
