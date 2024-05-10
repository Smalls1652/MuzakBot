using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Interface for services that interact with the Apple Music API.
/// </summary>
public interface IAppleMusicApiService
{
    Task<Artist[]> SearchArtistsAsync(string searchTerm);
    Task<Album[]> SearchAlbumsAsync(string searchTerm);
    Task<Song[]> SearchSongsAsync(string searchTerm);

    Task<Artist> GetArtistFromCatalogAsync(string id);
    Task<Album> GetAlbumFromCatalogAsync(string id);
    Task<Song> GetSongFromCatalogAsync(string id);
}