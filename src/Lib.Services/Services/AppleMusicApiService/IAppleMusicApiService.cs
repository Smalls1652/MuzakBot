using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Interface for services that interact with the Apple Music API.
/// </summary>
public interface IAppleMusicApiService
{
    Task<Artist[]> SearchArtistsAsync(string searchTerm);
}
