using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Logging.MusicBrainz;

/// <summary>
/// Source generated logging methods for the <see cref="Services.MusicBrainzService"/> class.
/// </summary>
public static partial class MusicBrainzServiceLogging
{
    /// <summary>
    /// Logs a search on the MusicBrainz API.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="searchQuery">The search query being used.</param>
    [LoggerMessage(
        EventName = "MusicBrainzApiService.Search.Start",
        Level = LogLevel.Information,
        Message = "Searching for '{searchQuery}' on the MusicBrainz API."
    )]
    public static partial void LogMusicBrainzApiServiceSearchStart(this ILogger logger, string searchQuery);

    /// <summary>
    /// Logs a failed search on the MusicBrainz API.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="exception">The exception that occurred.</param>
    [LoggerMessage(
        EventName = "MusicBrainzApiService.Search.Failure",
        Level = LogLevel.Error,
        Message = "An error occurred while making a request to the MusicBrainz API."
    )]
    public static partial void LogMusicBrainzApiServiceFailure(this ILogger logger, Exception exception);
}