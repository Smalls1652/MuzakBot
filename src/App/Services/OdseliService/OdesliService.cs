using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using MuzakBot;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Services;

/// <summary>
/// Service for interacting with the Odesli API.
/// </summary>
public class OdesliService : IOdesliService
{
    private readonly ILogger<OdesliService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public OdesliService(ILogger<OdesliService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Gets share links from the Odesli API for the given URL.
    /// </summary>
    /// <param name="inputUrl">The URL to get share links for.</param>
    /// <returns>Data from the Odesli API for the given URL.</returns>
    public async Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl)
    {
        var httpClient = _httpClientFactory.CreateClient("OdesliApiClient");

        _logger.LogInformation("Getting share links for '{inputUrl}'.", inputUrl);
        string encodedUrl = WebUtility.UrlEncode(inputUrl);

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"links?url={encodedUrl}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        responseMessage.EnsureSuccessStatusCode();

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();
        MusicEntityItem? musicEntityItem = await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: OdesliJsonContext.Default.MusicEntityItem
        );

        return musicEntityItem;
    }
}