using MuzakBot.App.Models.Itunes;

namespace MuzakBot.App.Services;

public interface IItunesApiService : IDisposable
{
    Task<ApiSearchResult<ArtistItem>?> GetArtistSearchResultAsync(string artistName);
    Task<ApiSearchResult<ArtistItem>?> GetArtistSearchResultAsync(string artistName, string? parentActvitityId);

    Task<ApiSearchResult<ArtistItem>?> GetArtistIdLookupResultAsync(string artistId);
    Task<ApiSearchResult<ArtistItem>?> GetArtistIdLookupResultAsync(string artistId, string? parentActvitityId);

    Task<ApiSearchResult<SongItem>?> GetSongsByArtistResultAsync(string artistName, string songName);
    Task<ApiSearchResult<SongItem>?> GetSongsByArtistResultAsync(string artistName, string songName, string? parentActvitityId);

    Task<ApiSearchResult<AlbumItem>?> GetAlbumsByArtistResultAsync(string artistName, string albumName);
    Task<ApiSearchResult<AlbumItem>?> GetAlbumsByArtistResultAsync(string artistName, string albumName, string? parentActvitityId);

    Task<ApiSearchResult<SongItem>?> GetSongIdLookupResultAsync(string trackId);
    Task<ApiSearchResult<SongItem>?> GetSongIdLookupResultAsync(string trackId, string? parentActvitityId);
}