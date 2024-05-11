using MuzakBot.Lib.Models.Genius;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Interface for the Genius API service.
/// </summary>
public interface IGeniusApiService : IDisposable
{
    Task<GeniusApiResponse<GeniusSearchResult>?> SearchAsync(string artistName, string songName);
    Task<GeniusApiResponse<GeniusSearchResult>?> SearchAsync(string artistName, string songName, string? parentActvitityId);

    Task<string> GetLyricsAsync(string url);
    Task<string> GetLyricsAsync(string url, string? parentActvitityId);

    Task<string> GetLyricsDirectlyAsync(string url);
    Task<string> GetLyricsDirectlyAsync(string url, string? parentActvitityId);
}
