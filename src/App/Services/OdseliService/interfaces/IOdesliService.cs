using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Services;

/// <summary>
/// Interface for the Odesli service.
/// </summary>
public interface IOdesliService
{
    Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl);
}