using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Extensions.GeniusApiService;

/// <summary>
/// Source generated logging methods for the <see cref="GeniusApiService"/> class.
/// </summary>
internal static partial class GeniusApiServiceLogging
{
    /// <summary>
    /// Logs a Genius API search.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="artistName">The name of the artist.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.SearchAsync",
        EventId = 10,
        Level = LogLevel.Information,
        Message = "Searching the Genius API for '{songName}' by '{artistName}'."
    )]
    public static partial void LogGeniusSearch(this ILogger logger, string songName, string artistName);

    /// <summary>
    /// Logs a request to get lyrics from Genius.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL of the lyrics page.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLyricsAsync",
        EventId = 11,
        Level = LogLevel.Information,
        Message = "Getting lyrics from '{url}'."
    )]
    public static partial void LogGeniusGetLyrics(this ILogger logger, string url);

    /// <summary>
    /// Logs a request to get the latest Wayback Machine snapshot for a URL.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLatestWaybackAsync",
        EventId = 20,
        Level = LogLevel.Information,
        Message = "Getting latest Wayback Machine snapshot for '{url}'."
    )]
    public static partial void LogGeniusGetLatestWayback(this ILogger logger, string url);

    /// <summary>
    /// Logs that no Wayback Machine snapshot was found for a URL.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLatestWaybackAsync.NotFound",
        EventId = 21,
        Level = LogLevel.Warning,
        Message = "No Wayback Machine snapshot found for '{url}'."
    )]
    public static partial void LogGeniusGetLatestWaybackNotFound(this ILogger logger, string url);

    /// <summary>
    /// Logs that an attempt to archive a URL by the Wayback Machine is being made.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLatestWaybackAsync.AttemptArchive",
        EventId = 22,
        Level = LogLevel.Information,
        Message = "Attempting to archive '{url}'..."
    )]
    public static partial void LogGeniusGetLatestWaybackAttemptArchive(this ILogger logger, string url);

    /// <summary>
    /// Logs that a URL could not be archived by the Wayback Machine.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLatestWaybackAsync.NotArchived",
        EventId = 23,
        Level = LogLevel.Warning,
        Message = "Could not archive '{url}'."
    )]
    public static partial void LogGeniusGetLatestWaybackNotArchived(this ILogger logger, string url);

    /// <summary>
    /// Logs that a URL was successfully archived by the Wayback Machine.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    /// <param name="archiveUrl">The URL of the archived page.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLatestWaybackAsync.ArchiveSuccess",
        EventId = 24,
        Level = LogLevel.Information,
        Message = "Successfully archived '{url}' to '{archiveUrl}'."
    )]
    public static partial void LogGeniusGetLatestWaybackArchiveSuccess(this ILogger logger, string url, string archiveUrl);

    /// <summary>
    /// Logs that the latest Wayback Machine snapshot was found for a URL.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    /// <param name="archiveUrl">The URL of the archived page.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.GetLatestWaybackAsync.LatestFound",
        EventId = 25,
        Level = LogLevel.Information,
        Message = "Found latest Wayback Machine snapshot for '{url}' at '{archiveUrl}'."
    )]
    public static partial void LogGeniusGetLatestWaybackLatestFound(this ILogger logger, string url, string archiveUrl);

    /// <summary>
    /// Logs that a request to archive a URL to the Wayback Machine is being made.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="url">The URL for the request.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.InvokeWaybackArchiveAsync",
        EventId = 30,
        Level = LogLevel.Information,
        Message = "Invoking Wayback Machine archive for '{url}'."
    )]
    public static partial void LogGeniusInvokeWaybackArchive(this ILogger logger, string url);

    /// <summary>
    /// Logs that the Wayback Machine response could not be deserialized.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.InvokeWaybackArchiveAsync.DeserializationError",
        EventId = 31,
        Level = LogLevel.Error,
        Message = "Could not deserialize Wayback Machine response."
    )]
    public static partial void LogGeniusInvokeWaybackArchiveDeserializationError(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs that the HTML for a lyrics page was not successfully parsed.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventName = "GeniusApiService.ParseLyricsHtml",
        EventId = 40,
        Level = LogLevel.Warning,
        Message = "Could not parse lyrics HTML."
    )]
    public static partial void LogGeniusParseLyricsHtmlNotParsed(this ILogger logger);
}