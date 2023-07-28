using MuzakBot.App.Models.Itunes;

namespace MuzakBot.App.Services;

public interface IItunesApiService
{
    Task<ApiSearchResult<ArtistItem>?> GetArtistSearchResultAsync(string artistName);
    Task<ApiSearchResult<ArtistItem>?> GetArtistIdLookupResultAsync(string artistId);
    Task<ApiSearchResult<SongItem>?> GetSongsByArtistResultAsync(ArtistItem artistItem, string songName);
    Task<ApiSearchResult<SongItem>?> GetSongIdLookupResultAsync(string trackId);
}