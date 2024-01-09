namespace MuzakBot.App.Models.Genius;

public interface IGeniusSearchResult
{
    GeniusSearchResultHitItem[]? Hits { get; set; }
}