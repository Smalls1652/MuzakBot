using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Services;

/// <summary>
/// Interface for the Odesli service.
/// </summary>
public interface IOdesliService : IDisposable
{
    Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl);
    Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl, string? parentActvitityId);
}