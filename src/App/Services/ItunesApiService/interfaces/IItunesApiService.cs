using MuzakBot.App.Models.Itunes;

namespace MuzakBot.App.Services;

public interface IItunesApiService
{
    Task<ApiSearchResult<ArtistItem>?> GetArtistSearchResultAsync(string artistName);
    Task<ApiSearchResult<ArtistItem>?> GetArtistIdLookupResultAsync(string artistId);
    Task<ApiSearchResult<SongItem>?> GetSongsByArtistResultAsync(string artistName, string songName);
    Task<ApiSearchResult<AlbumItem>?> GetAlbumsByArtistResultAsync(string artistName, string albumName);
    Task<ApiSearchResult<SongItem>?> GetSongIdLookupResultAsync(string trackId);
}