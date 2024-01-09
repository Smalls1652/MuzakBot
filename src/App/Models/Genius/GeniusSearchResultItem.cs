namespace MuzakBot.App.Models.Genius;

public class GeniusSearchResultItem : IGeniusSearchResultItem
{
    [JsonPropertyName("api_path")]
    public string? ApiPath { get; set; }

    [JsonPropertyName("full_title")]
    public string? FullTitle { get; set; }

    [JsonPropertyName("artist_names")]
    public string? ArtistNames { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("lyrics_state")]
    public string? LyricsState { get; set; }
}