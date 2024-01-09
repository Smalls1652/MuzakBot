using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Genius;
using MuzakBot.App.Models.Wayback;

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

        string? latestWaybackUrl = await GetLatestWayback(url);

        if (latestWaybackUrl is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw new InvalidOperationException("Could not find a Wayback Machine snapshot of the URL.");
        }

        using var client = _httpClientFactory.CreateClient("InternetArchiveClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: latestWaybackUrl
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

    private async Task<string?> GetLatestWayback(string url) => await GetLatestWayback(url, false);

    private async Task<string?> GetLatestWayback(string url, bool isSecondRun)
    {
        using var client = _httpClientFactory.CreateClient("InternetArchiveClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"wayback/available?url={url}&timestamp={DateTimeOffset.UtcNow:yyyyMMdd}"
        );

        requestMessage.Headers.Add("Accept", "application/json");

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        string responseContent = await responseMessage.Content.ReadAsStringAsync();

        _logger.LogInformation("{ResponseContent}", responseContent);

        ArchivedStatus? archivedStatus = JsonSerializer.Deserialize<ArchivedStatus>(responseContent);

        if (archivedStatus is null || archivedStatus.ArchivedSnapshots is null || archivedStatus.ArchivedSnapshots.Closest is null || archivedStatus.ArchivedSnapshots.Closest.Available == false || archivedStatus.ArchivedSnapshots.Closest.Url is null)
        {
            _logger.LogWarning("Could not find a Wayback Machine snapshot of the URL.");
            
            if (!isSecondRun)
            {
                _logger.LogInformation("Attempting to archive the URL.");

                bool archiveSuccess = await InvokeWaybackArchive(url);

                if (!archiveSuccess)
                {
                    _logger.LogWarning("Could not archive the URL.");
                    return null;
                }

                _logger.LogInformation("Successfully archived the URL.");

                return await GetLatestWayback(url, true);
            }
        }

        string latestWaybackUrl = archivedStatus!.ArchivedSnapshots!.Closest!.Url!;

        return latestWaybackUrl;
    }

    private async Task<bool> InvokeWaybackArchive(string url)
    {
        using var client = _httpClientFactory.CreateClient("InternetArchiveClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Post,
            requestUri: $"https://web.archive.org/save/{url}"            
        );

        MultipartFormDataContent formContent = new()
        {
            { new StringContent(url), "url" },
            { new StringContent("on"), "capture_all" }
        };

        requestMessage.Content = formContent;
        requestMessage.Content.Headers.ContentDisposition = new("form-data");

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        string responseContent = await responseMessage.Content.ReadAsStringAsync();

        if (!WaybackArchiveJobIdRegex().IsMatch(responseContent))
        {
            return false;
        }

        Match jobIdMatch = WaybackArchiveJobIdRegex().Match(responseContent);

        string jobId = jobIdMatch.Groups["jobId"].Value;

        TimeSpan timeout = TimeSpan.FromMinutes(5);

        Stopwatch stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            await Task.Delay(15000);

            using HttpRequestMessage jobRequestMessage = new(
                method: HttpMethod.Get,
                requestUri: $"https://web.archive.org/save/status/{jobId}"
            );

            using HttpResponseMessage jobResponseMessage = await client.SendAsync(jobRequestMessage);

            string jobResponseJson = await jobResponseMessage.Content.ReadAsStringAsync();

            SaveJobStatus? jobResponse = JsonSerializer.Deserialize<SaveJobStatus>(jobResponseJson);

            if (jobResponse is null)
            {
                return false;
            }

            if (jobResponse.Status == "success")
            {
                await Task.Delay(30000);
                return true;
            }
        }

        return false;
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

    [GeneratedRegex(
        pattern: "spn\\.watchJob\\(\"(?'jobId'.+?)\", .+?,.+?\\);",
        options: RegexOptions.Singleline
    )]
    private static partial Regex WaybackArchiveJobIdRegex();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}