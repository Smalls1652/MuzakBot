using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.Itunes;

public class ApiSearchResult<T> : IApiSearchResult<T>
{
    [JsonPropertyName("resultCount")]
    public int ResultCount { get; set; }

    [JsonPropertyName("results")]
    public T[]? Results { get; set; }
}