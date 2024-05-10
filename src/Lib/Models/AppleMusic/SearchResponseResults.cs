namespace MuzakBot.Lib.Models.AppleMusic;

public sealed class SearchResponseResults
{
    [JsonPropertyName("albums")]
    public SearchResponseResultsData<Album>? Albums { get; set; }

    [JsonPropertyName("artists")]
    public SearchResponseResultsData<Artist>? Artists { get; set; }

    [JsonPropertyName("songs")]
    public SearchResponseResultsData<Song>? Songs { get; set; }
}