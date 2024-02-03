using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.Itunes;

/// <summary>
/// Represents the result of an API search in iTunes.
/// </summary>
/// <typeparam name="T">The type of the search results.</typeparam>
public class ApiSearchResult<T> : IApiSearchResult<T>
{
    /// <summary>
    /// Gets or sets the number of results returned by the API search.
    /// </summary>
    [JsonPropertyName("resultCount")]
    public int ResultCount { get; set; }

    /// <summary>
    /// Gets or sets the array of search results.
    /// </summary>
    [JsonPropertyName("results")]
    public T[]? Results { get; set; }
}