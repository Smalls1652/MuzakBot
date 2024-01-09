namespace MuzakBot.App.Models.Genius;

public class GeniusSearchResultHitItem : IGeniusSearchResultHitItem
{
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("result")]
    public GeniusSearchResultItem? Result { get; set; }
}