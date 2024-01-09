using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Genius;

namespace MuzakBot.App.Services;

/// <summary>
/// Service for interacting with the Genius API and retrieving song lyrics.
/// </summary>
public partial class GeniusApiService : IGeniusApiService
{
    private bool _isDisposed;
    private readonly ILogger<GeniusApiService> _logger;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.GeniusApiService");
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _accessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeniusApiService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
    /// <param name="options">The <see cref="GeniusApiServiceOptions"/> for configuring the service.</param>
    public GeniusApiService(ILogger<GeniusApiService> logger, IHttpClientFactory httpClientFactory, IOptions<GeniusApiServiceOptions> options)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _accessToken = options.Value.AccessToken;
    }

    /// <summary>
    /// Searches for a song by artist name and song name using the Genius API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The search results from the API.</returns>
    public async Task<GeniusApiResponse<GeniusSearchResult>?> SearchAsync(string artistName, string songName) => await SearchAsync(artistName, songName, null);


    /// <summary>
    /// Searches for a song by artist name and song name using the Genius API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActvitityId">The ID of the parent activity (optional).</param>
    /// <returns>The search results from the API.</returns>
    public async Task<GeniusApiResponse<GeniusSearchResult>?> SearchAsync(string artistName, string songName, string? parentActvitityId)
    {
        using var activity = _activitySource.StartGeniusSearchAsyncActivity(artistName, songName, parentActvitityId);

        using var client = _httpClientFactory.CreateClient("GeniusApiClient");

        string encodedSearchQuery = HttpUtility.UrlPathEncode($"{artistName} {songName}");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"search?q={encodedSearchQuery}"
        );

        requestMessage.Headers.Authorization = new("Bearer", _accessToken);

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        Stream responseContent = await responseMessage.Content.ReadAsStreamAsync();

        GeniusApiResponse<GeniusSearchResult>? searchResult = await JsonSerializer.DeserializeAsync(
            utf8Json: responseContent,
            jsonTypeInfo: GeniusJsonContext.Default.GeniusApiResponseGeniusSearchResult
        );

        return searchResult;
    }

    /// <summary>
    /// Retrieves the lyrics of a song from a given URL.
    /// </summary>
    /// <remarks>
    /// This scrapes the webpage to retrieve the lyrics.
    /// </remarks>
    /// <param name="url">The URL of the song lyrics.</param>
    /// <returns>The lyrics of the song as a string.</returns>
    public async Task<string> GetLyricsAsync(string url) => await GetLyricsAsync(url, null);

    /// <summary>
    /// Retrieves the lyrics of a song from a given URL.
    /// </summary>
    /// <remarks>
    /// This scrapes the webpage to retrieve the lyrics.
    /// </remarks>
    /// <param name="url">The URL of the song lyrics.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>The lyrics of the song as a string.</returns>
    public async Task<string> GetLyricsAsync(string url, string? parentActvitityId)
    {
        using var activity = _activitySource.StartGeniusGetLyricsAsyncActivity(url, parentActvitityId);

        using var client = _httpClientFactory.CreateClient("GeniusClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: url
        );

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        string webpageContent = await responseMessage.Content.ReadAsStringAsync();

        string? lyrics = ParseLyricsHtml(webpageContent);

        if (lyrics is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw new InvalidOperationException("Could not parse lyrics from HTML.");
        }

        return lyrics;
    }

    /// <summary>
    /// Parses the HTML content and extracts the lyrics from it.
    /// </summary>
    /// <param name="html">The HTML content containing the lyrics.</param>
    /// <returns>The extracted lyrics as a string, or null if the HTML does not contain any lyrics.</returns>
    private static string? ParseLyricsHtml(string html)
    {
        if (!LyricsContainerRegex().IsMatch(html))
        {
            return null;
        }

        MatchCollection containerMatches = LyricsContainerRegex().Matches(html);

        if (containerMatches.Count == 0)
        {
            return null;
        }

        StringBuilder allLyricsContainersStringBuilder = new();

        foreach (Match containerMatchItem in containerMatches)
        {
            allLyricsContainersStringBuilder.AppendLine(containerMatchItem.Groups["lyricsContent"].Value);
        }

        string allLyricsContainers = allLyricsContainersStringBuilder.ToString();

        allLyricsContainers = allLyricsContainers.Replace("<br/>", Environment.NewLine);
        allLyricsContainers = HtmlElementRegex().Replace(allLyricsContainers, "");
        allLyricsContainers = HttpUtility.HtmlDecode(allLyricsContainers);

        return allLyricsContainers;
    }

    [GeneratedRegex(
        pattern: @"<div data-lyrics-container=.+?>(?'lyricsContent'(?s).+?)<\/div>"
    )]
    private static partial Regex LyricsContainerRegex();

    [GeneratedRegex(
        pattern: @"(<(?:\/|).+?>)"
    )]
    private static partial Regex HtmlElementRegex();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}