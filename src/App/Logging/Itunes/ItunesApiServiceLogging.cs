using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Logging.Itunes;

public static partial class ItunesApiServiceLogging
{
    /// <summary>
    /// Logs a search on the iTunes API.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="searchQuery">The search query being used.</param>
    [LoggerMessage(
        EventName = "iTunesApiService.Search.Start",
        Level = LogLevel.Information,
        Message = "Searching for '{searchQuery}' on the iTunes API."
    )]
    public static partial void LogItunesApiServiceSearchStart(this ILogger logger, string searchQuery);

    /// <summary>
    /// Logs a failed search on the iTunes API.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="exception">The exception that occurred.</param>
    [LoggerMessage(
        EventName = "iTunesApiService.Search.Failure",
        Level = LogLevel.Error,
        Message = "An error occurred while making a request to the iTunes API."
    )]
    public static partial void LogItunesApiServiceFailure(this ILogger logger, Exception exception);
}