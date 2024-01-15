using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public interface ICosmosDbService : IDisposable
{
    Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync();
    Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync(string? parentActivityId);

    Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig);
    Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig, string? parentActivityId);

    Task<LyricsAnalyzerUserRateLimit> GetLyricsAnalyzerUserRateLimitAsync(ulong userId);
    Task<LyricsAnalyzerUserRateLimit> GetLyricsAnalyzerUserRateLimitAsync(ulong userId, string? parentActivityId);

    Task AddOrUpdateLyricsAnalyzerUserRateLimitAsync(LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit);
    Task AddOrUpdateLyricsAnalyzerUserRateLimitAsync(LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit, string? parentActivityId);

    Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem);
    Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem, string? parentActivityId);

    Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName);
    Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName, string? parentActivityId);

    Task AddOrUpdateLyricsAnalyzerPromptStyleAsync(LyricsAnalyzerPromptStyle lyricsAnalyzerPromptStyle);
    Task AddOrUpdateLyricsAnalyzerPromptStyleAsync(LyricsAnalyzerPromptStyle lyricsAnalyzerPromptStyle, string? parentActivityId);

    Task<LyricsAnalyzerPromptStyle> GetLyricsAnalyzerPromptStyleAsync(string shortName);
    Task<LyricsAnalyzerPromptStyle> GetLyricsAnalyzerPromptStyleAsync(string shortName, string? parentActivityId);

    Task<LyricsAnalyzerPromptStyle[]> GetLyricsAnalyzerPromptStylesAsync();
    Task<LyricsAnalyzerPromptStyle[]> GetLyricsAnalyzerPromptStylesAsync(string? parentActivityId);
}