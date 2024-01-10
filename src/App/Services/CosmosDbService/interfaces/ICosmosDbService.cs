using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public interface ICosmosDbService : IDisposable
{
    Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync();
    Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync(string? parentActivityId);

    Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig);
    Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig, string? parentActivityId);

    Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem);
    Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem, string? parentActivityId);

    Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName);
    Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName, string? parentActivityId);
}