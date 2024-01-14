using MuzakBot.App.Models.Genius;

namespace MuzakBot.App.Services;

/// <summary>
/// Interface for the Genius API service.
/// </summary>
public interface IGeniusApiService : IDisposable
{
    Task<GeniusApiResponse<GeniusSearchResult>?> SearchAsync(string artistName, string songName);
    Task<GeniusApiResponse<GeniusSearchResult>?> SearchAsync(string artistName, string songName, string? parentActvitityId);

    Task<string> GetLyricsAsync(string url);
    Task<string> GetLyricsAsync(string url, string? parentActvitityId);
}