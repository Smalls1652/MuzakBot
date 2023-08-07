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
}