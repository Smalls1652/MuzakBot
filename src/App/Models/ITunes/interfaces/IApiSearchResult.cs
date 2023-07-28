namespace MuzakBot.App.Models.Itunes;

public interface IApiSearchResult<T>
{
    int ResultCount { get; set; }
    T[]? Results { get; set; }
}