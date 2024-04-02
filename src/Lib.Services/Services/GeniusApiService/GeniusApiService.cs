using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MuzakBot.Lib.Models.Genius;
using MuzakBot.Lib.Models.Wayback;
using MuzakBot.Lib.Services.Extensions.Telemetry;
using MuzakBot.Lib.Services.Logging.Genius;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service for interacting with the Genius API and retrieving song lyrics.
/// </summary>
public partial class GeniusApiService : IGeniusApiService
{
    private bool _isDisposed;
    private readonly ILogger<GeniusApiService> _logger;
    private readonly ActivitySource _activitySource = new("MuzakBot.Lib.Services.GeniusApiService");
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

        _logger.LogGeniusSearch(songName, artistName);

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

    public async Task<string> GetLyricsDirectlyAsync(string url) => await GetLyricsDirectlyAsync(url, null);

    public async Task<string> GetLyricsDirectlyAsync(string url, string? parentActvitityId)
    {
        using var activity = _activitySource.StartActivity(
            name: "GetLyricsDirectlyAsync",
            kind: ActivityKind.Client,
            tags: new ActivityTagsCollection
            {
                { "url", url }
            },
            parentId: parentActvitityId
        );

        using var client = _httpClientFactory.CreateClient("GeniusClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: url
        );

        requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");

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

        _logger.LogGeniusGetLyrics(url);

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

    /// <summary>
    /// Gets the latest Wayback Machine snapshot of the URL.
    /// </summary>
    /// <param name="url">The URL to get the latest Wayback Machine snapshot of.</param>
    /// <returns>The URL of the latest Wayback Machine snapshot.</returns>
    private async Task<string?> GetLatestWayback(string url) => await GetLatestWayback(url, false);

    /// <summary>
    /// Gets the latest Wayback Machine snapshot of the URL.
    /// </summary>
    /// <param name="url">The URL to get the latest Wayback Machine snapshot of.</param>
    /// <param name="isSecondRun">Whether or not this is the second run of the method.</param>
    /// <returns>The URL of the latest Wayback Machine snapshot.</returns>
    private async Task<string?> GetLatestWayback(string url, bool isSecondRun)
    {
        _logger.LogGeniusGetLatestWayback(url);

        using var client = _httpClientFactory.CreateClient("InternetArchiveClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"wayback/available?url={url}&timestamp={DateTimeOffset.UtcNow:yyyyMMdd}"
        );

        requestMessage.Headers.Add("Accept", "application/json");

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        string responseContent = await responseMessage.Content.ReadAsStringAsync();

        ArchivedStatus? archivedStatus = JsonSerializer.Deserialize<ArchivedStatus>(responseContent);

        if (archivedStatus is null || archivedStatus.ArchivedSnapshots is null || archivedStatus.ArchivedSnapshots.Closest is null || archivedStatus.ArchivedSnapshots.Closest.Available == false || ConvertWaybackTimeStampToDateTimeOffset(archivedStatus.ArchivedSnapshots.Closest.Timestamp) < DateTimeOffset.UtcNow.AddDays(-30))
        {
            _logger.LogGeniusGetLatestWaybackNotFound(url);
            
            if (!isSecondRun)
            {
                _logger.LogGeniusGetLatestWaybackAttemptArchive(url);

                string newArchiveUrl;
                try
                {
                    newArchiveUrl = await InvokeWaybackArchive(url);
                }
                catch (Exception)
                {
                    _logger.LogGeniusGetLatestWaybackNotArchived(url);
                    return null;
                }

                _logger.LogGeniusGetLatestWaybackArchiveSuccess(url, newArchiveUrl);

                return newArchiveUrl;
            }
        }

        string latestWaybackUrl = archivedStatus!.ArchivedSnapshots!.Closest!.Url!;

        _logger.LogGeniusGetLatestWaybackLatestFound(url, latestWaybackUrl);

        return latestWaybackUrl;
    }

    /// <summary>
    /// Invokes the Wayback Machine to archive the URL.
    /// </summary>
    /// <param name="url">The URL to archive.</param>
    /// <returns>The URL of the archived page.</returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="TimeoutException"></exception>
    private async Task<string> InvokeWaybackArchive(string url)
    {
        _logger.LogGeniusInvokeWaybackArchive(url);

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
            throw new Exception("Could not find job ID in response.");
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
                Exception deserializationException = new("Could not deserialize response from Wayback Machine.");

                _logger.LogGeniusInvokeWaybackArchiveDeserializationError(deserializationException);
                throw deserializationException;
            }

            if (jobResponse.Status == "success" && jobResponse.Timestamp is not null)
            {
                return $"https://web.archive.org/web/{jobResponse.Timestamp}/{url}";
            }
        }

        throw new TimeoutException("The Wayback Machine archive job timed out.");
    }

    /// <summary>
    /// Converts a Wayback Machine timestamp to a <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to convert.</param>
    /// <returns>The converted timestamp as a <see cref="DateTimeOffset"/>.</returns>
    private DateTimeOffset? ConvertWaybackTimeStampToDateTimeOffset(string timestamp)
    {
        if (!WaybackTimestampRegex().IsMatch(timestamp))
        {
            return null;
        }

        Match timestampMatch = WaybackTimestampRegex().Match(timestamp);

        int year = int.Parse(timestampMatch.Groups["year"].Value);
        int month = int.Parse(timestampMatch.Groups["month"].Value);
        int day = int.Parse(timestampMatch.Groups["day"].Value);
        int hour = int.Parse(timestampMatch.Groups["hour"].Value);
        int minute = int.Parse(timestampMatch.Groups["minute"].Value);
        int second = int.Parse(timestampMatch.Groups["second"].Value);

        return new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);
    }

    /// <summary>
    /// Parses the HTML content and extracts the lyrics from it.
    /// </summary>
    /// <param name="html">The HTML content containing the lyrics.</param>
    /// <returns>The extracted lyrics as a string, or null if the HTML does not contain any lyrics.</returns>
    private string? ParseLyricsHtml(string html)
    {
        if (!LyricsContainerRegex().IsMatch(html))
        {
            _logger.LogGeniusParseLyricsHtmlNotParsed();
            return null;
        }

        MatchCollection containerMatches = LyricsContainerRegex().Matches(html);

        if (containerMatches.Count == 0)
        {
            _logger.LogGeniusParseLyricsHtmlNotParsed();
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

    [GeneratedRegex(
        pattern: @"(?'year'\d{4})(?'month'\d{2})(?'day'\d{2})(?'hour'\d{2})(?'minute'\d{2})(?'second'\d{2})"
    )]
    private static partial Regex WaybackTimestampRegex();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}