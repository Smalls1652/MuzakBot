namespace MuzakBot.App.Models.Genius;

public interface IGeniusSearchResultItem
{
    string? ApiPath { get; set; }
    string? FullTitle { get; set; }
    string? ArtistNames { get; set; }
    int Id { get; set; }
    string? Title { get; set; }
    string? Url { get; set; }
    string? Path { get; set; }
    string? LyricsState { get; set; }
}