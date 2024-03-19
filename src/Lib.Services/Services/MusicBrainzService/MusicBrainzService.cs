using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Services.Logging.MusicBrainz;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.Lib.Models.Diagnostics;
using MuzakBot.Lib.Services.Extensions.Telemetry;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service for interacting with the MusicBrainz API.
/// </summary>
public partial class MusicBrainzService : IMusicBrainzService
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.Lib.Services.MusicBrainzService");
    private readonly ILogger<IMusicBrainzService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicBrainzService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public MusicBrainzService(ILogger<IMusicBrainzService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Searches for an artist with the given name.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <returns>The artist.</returns>
    public async Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName) => await SearchArtistAsync(artistName, null);

    /// <summary>
    /// Searches for an artist with the given name.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The search result of artists.</returns>
    public async Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.SearchArtist,
            tags: new()
            {
                ArtistName = artistName
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart(artistName);

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"artist/?query={artistName}&limit=5"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzArtistSearchResult
        );
    }

    /// <summary>
    /// Searches for recordings of a specific artist and song.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The search result of artist recordings.</returns>
    public async Task<MusicBrainzRecordingSearchResult?> SearchArtistRecordingsAsync(string artistId, string songName) => await SearchArtistRecordingsAsync(artistId, songName, null);

    /// <summary>
    /// Searches for recordings of a specific artist and song.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The search result of artist recordings.</returns>
    public async Task<MusicBrainzRecordingSearchResult?> SearchArtistRecordingsAsync(string artistId, string songName, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.SearchArtistRecordings,
            tags: new()
            {
                ArtistId = artistId,
                SongName = songName
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart($"{artistId} - {songName}");

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"recording/?query=arid:{artistId} AND {WebUtility.UrlEncode(songName)}&inc=releases&limit=5"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzRecordingSearchResult
        );
    }

    /// <summary>
    /// Searches for recordings of a specific artist and song.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The search result of artist recordings.</returns>
    public async Task<MusicBrainzRecordingSearchResult?> SearchArtistByNameRecordingsAsync(string artistName, string songName) => await SearchArtistByNameRecordingsAsync(artistName, songName, null);

    /// <summary>
    /// Searches for recordings of a specific artist and song.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The search result of artist recordings.</returns>
    public async Task<MusicBrainzRecordingSearchResult?> SearchArtistByNameRecordingsAsync(string artistName, string songName, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.SearchArtistRecordings,
            tags: new()
            {
                ArtistName = artistName,
                SongName = songName
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart($"{artistName} - {songName}");

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"recording/?query=area:{WebUtility.UrlEncode(artistName)} AND {WebUtility.UrlEncode(songName)}&inc=releases&limit=5"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzRecordingSearchResult
        );
    }

    /// <summary>
    /// Searches for releases by a given artist and album name.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="albumName">The name of the album.</param>
    /// <returns>The search result of artist releases.</returns>
    public async Task<MusicBrainzReleaseSearchResult?> SearchArtistReleasesAsync(string artistId, string albumName) => await SearchArtistReleasesAsync(artistId, albumName, null);

    /// <summary>
    /// Searches for releases by a given artist and album name.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="albumName">The name of the album.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The search result of artist releases.</returns>
    public async Task<MusicBrainzReleaseSearchResult?> SearchArtistReleasesAsync(string artistId, string albumName, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.SearchArtistReleases,
            tags: new()
            {
                ArtistId = artistId,
                AlbumName = albumName
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart($"{artistId} - {albumName}");

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"release/?query=arid:{artistId} AND {WebUtility.UrlEncode(albumName)}&limit=5"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzReleaseSearchResult
        );
    }

    /// <summary>
    /// Lookup an artist using the specified artist ID.
    /// </summary>
    /// <param name="artistId">The ID of the artist to lookup.</param>
    /// <returns>The found artist.</returns>
    public async Task<MusicBrainzArtistItem?> LookupArtistAsync(string artistId) => await LookupArtistAsync(artistId, null);

    /// <summary>
    /// Lookup an artist using the specified artist ID.
    /// </summary>
    /// <param name="artistId">The ID of the artist to lookup.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The found artist.</returns>
    public async Task<MusicBrainzArtistItem?> LookupArtistAsync(string artistId, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.LookupArtist,
            tags: new()
            {
                ArtistId = artistId
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart(artistId);

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"artist/{artistId}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzArtistItem
        );
    }

    /// <summary>
    /// Lookup a release item.
    /// </summary>
    /// <param name="releaseId">The ID of the release item to lookup.</param>
    /// <returns>The found release.</returns>
    public async Task<MusicBrainzReleaseItem?> LookupReleaseAsync(string releaseId) => await LookupReleaseAsync(releaseId, null);

    /// <summary>
    /// Lookup a release item.
    /// </summary>
    /// <param name="releaseId">The ID of the release item to lookup.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The found release.</returns>
    public async Task<MusicBrainzReleaseItem?> LookupReleaseAsync(string releaseId, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.LookupRelease,
            tags: new()
            {
                ReleaseId = releaseId
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart(releaseId);

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"release/{releaseId}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzReleaseItem
        );
    }

    /// <summary>
    /// Lookup a recording.
    /// </summary>
    /// <param name="recordingId">The ID of the recording to lookup.</param>
    /// <returns>The found recording.</returns>
    public async Task<MusicBrainzRecordingItem?> LookupRecordingAsync(string recordingId) => await LookupRecordingAsync(recordingId, null);

    /// <summary>
    /// Lookup a recording.
    /// </summary>
    /// <param name="recordingId">The ID of the recording to lookup.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The found recording.</returns>
    public async Task<MusicBrainzRecordingItem?> LookupRecordingAsync(string recordingId, string? parentActivityId)
    {
        using var activity = _activitySource.StartMusicBrainzServiceActivity(
            activityType: MusicBrainzActivityType.LookupRecording,
            tags: new()
            {
                RecordingId = recordingId
            },
            parentActivityId: parentActivityId
        );

        _logger.LogMusicBrainzApiServiceSearchStart(recordingId);

        using var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"recording/{recordingId}?inc=releases"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogMusicBrainzApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzRecordingItem
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