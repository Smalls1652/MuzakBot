using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Logging.Itunes;
using MuzakBot.App.Models.Diagnostics;
using MuzakBot.App.Models.Itunes;

namespace MuzakBot.App.Services;

/// <summary>
/// Represents a service for interacting with the iTunes API.
/// </summary>
public partial class ItunesApiService : IItunesApiService
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.ItunesApiService");
    private readonly ILogger<ItunesApiService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItunesApiService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public ItunesApiService(ILogger<ItunesApiService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Retrieves the search result for an artist from the iTunes API asynchronously.
    /// </summary>
    /// <param name="artistName">The name of the artist to search for.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the search result for the artist.</returns>
    public async Task<ApiSearchResult<ArtistItem>?> GetArtistSearchResultAsync(string artistName) => await GetArtistSearchResultAsync(artistName, null);

    /// <summary>
    /// Retrieves the search result for an artist from the iTunes API.
    /// </summary>
    /// <param name="artistName">The name of the artist to search for.</param>
    /// <param name="parentActvitityId">The ID of the parent activity, if any.</param>
    /// <returns>The search result for the artist.</returns>
    public async Task<ApiSearchResult<ArtistItem>?> GetArtistSearchResultAsync(string artistName, string? parentActvitityId)
    {
        using var activity = _activitySource.StartItunesApiServiceActivity(
            activityType: ItunesApiActivityType.GetArtitstSearchResult,
            tags: new()
            {
                ArtistName = artistName
            },
            parentActivityId: parentActvitityId
        );

        _logger.LogItunesApiServiceSearchStart(artistName);

        using var httpClient = _httpClientFactory.CreateClient("ItunesApiClient");

        string encodedSearch = WebUtility.UrlEncode(artistName);

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"search?country=US&media=music&entity=musicArtist&attribute=artistTerm&limit=5&term={encodedSearch}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogItunesApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: ItunesJsonContext.Default.ApiSearchResultArtistItem
        );
    }

    /// <summary>
    /// Retrieves the search result for an artist ID lookup asynchronously.
    /// </summary>
    /// <param name="artistId">The ID of the artist to lookup.</param>
    /// <returns>The search result for the artist ID lookup, or null if not found.</returns>
    public async Task<ApiSearchResult<ArtistItem>?> GetArtistIdLookupResultAsync(string artistId) => await GetArtistIdLookupResultAsync(artistId, null);

    /// <summary>
    /// Retrieves the lookup result for an artist by their ID from the iTunes API.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>The API search result containing the artist item, or null if not found.</returns>
    public async Task<ApiSearchResult<ArtistItem>?> GetArtistIdLookupResultAsync(string artistId, string? parentActvitityId)
    {
        using var activity = _activitySource.StartItunesApiServiceActivity(
            activityType: ItunesApiActivityType.GetArtistIdLookupResult,
            tags: new()
            {
                ArtistId = artistId
            },
            parentActivityId: parentActvitityId
        );

        _logger.LogItunesApiServiceSearchStart(artistId);

        using var httpClient = _httpClientFactory.CreateClient("ItunesApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"lookup?id={artistId}&entity=musicArtist"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogItunesApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: ItunesJsonContext.Default.ApiSearchResultArtistItem
        );
    }

    /// <summary>
    /// Retrieves the search result for a specific song ID asynchronously.
    /// </summary>
    /// <param name="trackId">The ID of the song to search for.</param>
    /// <returns>The search result for the specified song ID, or null if not found.</returns>
    public async Task<ApiSearchResult<SongItem>?> GetSongIdLookupResultAsync(string trackId) => await GetSongIdLookupResultAsync(trackId, null);

    /// <summary>
    /// Retrieves the search result for a specific song ID asynchronously.
    /// </summary>
    /// <param name="trackId">The ID of the track to search for.</param>
    /// <param name="parentActvitityId">The ID of the parent activity, if any.</param>
    /// <returns>The search result for the specified song ID, or null if not found.</returns>
    public async Task<ApiSearchResult<SongItem>?> GetSongIdLookupResultAsync(string trackId, string? parentActvitityId)
    {
        using var activity = _activitySource.StartItunesApiServiceActivity(
            activityType: ItunesApiActivityType.GetSongIdLookupResult,
            tags: new()
            {
                TrackId = trackId
            },
            parentActivityId: parentActvitityId
        );

        _logger.LogItunesApiServiceSearchStart(trackId);

        using var httpClient = _httpClientFactory.CreateClient("ItunesApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"lookup?id={trackId}&entity=song"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogItunesApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: ItunesJsonContext.Default.ApiSearchResultSongItem
        );
    }

    /// <summary>
    /// Retrieves a list of songs by the specified artist and song name from the iTunes API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The search result containing the song items.</returns>
    public async Task<ApiSearchResult<SongItem>?> GetSongsByArtistResultAsync(string artistName, string songName) => await GetSongsByArtistResultAsync(artistName, songName, null);

    /// <summary>
    /// Retrieves a list of songs by the specified artist and song name from the iTunes API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>The search result containing the song items.</returns>
    public async Task<ApiSearchResult<SongItem>?> GetSongsByArtistResultAsync(string artistName, string songName, string? parentActvitityId)
    {
        using var activity = _activitySource.StartItunesApiServiceActivity(
            activityType: ItunesApiActivityType.GetSongsByArtistResult,
            tags: new()
            {
                ArtistName = artistName,
                SongName = songName
            },
            parentActivityId: parentActvitityId
        );

        _logger.LogItunesApiServiceSearchStart($"{artistName} - {songName}");

        using var httpClient = _httpClientFactory.CreateClient("ItunesApiClient");

        string encodedSearch = WebUtility.UrlEncode($"{artistName} {songName}");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"search?country=US&media=music&entity=song&limit=5&term={encodedSearch}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogItunesApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: ItunesJsonContext.Default.ApiSearchResultSongItem
        );
    }

    /// <summary>
    /// Retrieves a list of album search results for a given artist and album name.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="albumName">The name of the album.</param>
    /// <returns>The album search result.</returns>
    public async Task<ApiSearchResult<AlbumItem>?> GetAlbumsByArtistResultAsync(string artistName, string albumName) => await GetAlbumsByArtistResultAsync(artistName, albumName, null);

    /// <summary>
    /// Retrieves a list of album search results for a given artist and album name.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="albumName">The name of the album.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>The album search result.</returns>
    public async Task<ApiSearchResult<AlbumItem>?> GetAlbumsByArtistResultAsync(string artistName, string albumName, string? parentActvitityId)
    {
        using var activity = _activitySource.StartItunesApiServiceActivity(
            activityType: ItunesApiActivityType.GetAlbumsByArtistResult,
            tags: new()
            {
                ArtistName = artistName,
                AlbumName = albumName
            },
            parentActivityId: parentActvitityId
        );

        _logger.LogItunesApiServiceSearchStart($"{artistName} - {albumName}");

        using var httpClient = _httpClientFactory.CreateClient("ItunesApiClient");

        string encodedSearch = WebUtility.UrlEncode($"{artistName} {albumName}");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"search?country=US&media=music&entity=album&limit=5&term={encodedSearch}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogItunesApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: ItunesJsonContext.Default.ApiSearchResultAlbumItem
        );
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}