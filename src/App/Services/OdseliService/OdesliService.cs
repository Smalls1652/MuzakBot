using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;
using MuzakBot;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Services;

public class OdesliService : IOdesliService
{
    private readonly ILogger<OdesliService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public OdesliService(ILogger<OdesliService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

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

        using var contentStream = await responseMessage.Content.ReadAsStreamAsync();
        MusicEntityItem? musicEntityItem = await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: OdesliJsonContext.Default.MusicEntityItem
        );

        return musicEntityItem;
    }
}