namespace MuzakBot.Lib.Models.Diagnostics;

/// <summary>
/// Represents the type of activity being performed by the <see cref="ItunesApiService"/>.
/// </summary>
public enum ItunesApiActivityType
{
    /// <summary>
    /// Represents the activity of running <see cref="ItunesApiService.GetArtistSearchResultAsync(string)"/>.
    /// </summary>
    GetArtitstSearchResult,

    /// <summary>
    /// Represents the activity of running <see cref="ItunesApiService.GetArtistIdLookupResultAsync(string)"/>.
    /// </summary>
    GetArtistIdLookupResult,

    /// <summary>
    /// Represents the activity of running <see cref="ItunesApiService.GetSongsByArtistResultAsync(string, string)"/>.
    /// </summary>
    GetSongsByArtistResult,

    /// <summary>
    /// Represents the activity of running <see cref="ItunesApiService.GetAlbumsByArtistResultAsync(string, string)"/>.
    /// </summary>
    GetAlbumsByArtistResult,

    /// <summary>
    /// Represents the activity of running <see cref="ItunesApiService.GetSongIdLookupResultAsync(string)"/>.
    /// </summary>
    GetSongIdLookupResult
}
