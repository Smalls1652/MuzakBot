using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public interface ICosmosDbService : IDisposable
{
    Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem);
    Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem, string? parentActivityId);

    Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName);
    Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName, string? parentActivityId);
}