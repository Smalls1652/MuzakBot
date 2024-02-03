namespace MuzakBot.Lib.Models.Genius;

public interface IGeniusSearchResult
{
    GeniusSearchResultHitItem[]? Hits { get; set; }
}