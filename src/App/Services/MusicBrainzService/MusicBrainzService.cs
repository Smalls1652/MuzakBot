using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.MusicBrainz;

namespace MuzakBot.App.Services;

public partial class MusicBrainzService : IMusicBrainzService
{
    private readonly ILogger<IMusicBrainzService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public MusicBrainzService(ILogger<IMusicBrainzService?> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName)
    {
        var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"artist/?query={artistName}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        responseMessage.EnsureSuccessStatusCode();

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzArtistSearchResult
        );
    }

    public async Task<MusicBrainzRecordingSearchResult?> SearchArtistRecordingsAsync(string artistName, string songName)
    {
        var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        MusicBrainzArtistItem? artistItem = await LookupArtistAsync(artistName);
        if (artistItem is null)
        {
            return null;
        }

        string encodedQuery = WebUtility.UrlEncode($"artist:{artistItem.Name} AND {songName}");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"recording/?query={encodedQuery}&inc=releases"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        responseMessage.EnsureSuccessStatusCode();

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzRecordingSearchResult
        );
    }

    public async Task<MusicBrainzArtistItem?> LookupArtistAsync(string artistId)
    {
        var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"artist/{artistId}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        responseMessage.EnsureSuccessStatusCode();

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzArtistItem
        );
    }

    public async Task<MusicBrainzReleaseItem?> LookupReleaseAsync(string releaseId)
    {
        var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"release/{releaseId}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        responseMessage.EnsureSuccessStatusCode();

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzReleaseItem
        );
    }

    public async Task<MusicBrainzRecordingItem?> LookupRecordingAsync(string recordingId)
    {
        var httpClient = _httpClientFactory.CreateClient("MusicBrainzApiClient");

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"recording/{recordingId}?inc=releases"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        responseMessage.EnsureSuccessStatusCode();

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: MusicBrainzJsonContext.Default.MusicBrainzRecordingItem
        );
    }
}