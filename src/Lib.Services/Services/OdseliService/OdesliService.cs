using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;

using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

using MuzakBot;
using MuzakBot.Lib.Models.Odesli;
using MuzakBot.Lib.Services.Extensions.Telemetry;
using MuzakBot.Lib.Services.Logging.Odesli;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service for interacting with the Odesli API.
/// </summary>
public partial class OdesliService : IOdesliService
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.Lib.Services.OdesliService");
    private readonly ILogger<OdesliService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OdesliService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public OdesliService(ILogger<OdesliService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Gets share links from the Odesli API for the given URL.
    /// </summary>
    /// <param name="inputUrl">The URL to get share links for.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>Data from the Odesli API for the given URL.</returns>
    public async Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl) => await GetShareLinksAsync(inputUrl, null);

    /// <summary>
    /// Gets share links from the Odesli API for the given URL.
    /// </summary>
    /// <param name="inputUrl">The URL to get share links for.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>Data from the Odesli API for the given URL.</returns>
    public async Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl, string? parentActvitityId)
    {
        using var activity = _activitySource.StartGetShareLinksActivity(inputUrl, parentActvitityId);

        _logger.LogOdesliApiServiceShareLinksStart(inputUrl);

        using var httpClient = _httpClientFactory.CreateClient("OdesliApiClient");

        string encodedUrl = WebUtility.UrlEncode(inputUrl);

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"links?url={encodedUrl}"
        );

        var responseMessage = await httpClient.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogOdesliApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();
        MusicEntityItem? musicEntityItem = await JsonSerializer.DeserializeAsync(
            utf8Json: contentStream,
            jsonTypeInfo: OdesliJsonContext.Default.MusicEntityItem
        );

        return musicEntityItem;
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
