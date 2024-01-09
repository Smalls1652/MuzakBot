namespace MuzakBot.App.Models.Genius;

public interface IGeniusSearchResultHitItem
{
    string? Index { get; set; }
    string? Type { get; set; }
    GeniusSearchResultItem? Result { get; set; }
}