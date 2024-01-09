namespace MuzakBot.App.Models.Genius;

public class GeniusSearchResult : IGeniusSearchResult
{
    [JsonPropertyName("hits")]
    public GeniusSearchResultHitItem[]? Hits { get; set; }
}