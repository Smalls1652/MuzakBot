using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Logging.Odesli;

/// <summary>
/// Source generated logging methods for the <see cref="Services.OdesliService"/> class.
/// </summary>
public static partial class OdesliServiceLogging
{
    /// <summary>
    /// Logs a search on the Odesli API.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="shareUrl">The search query being used.</param>
    [LoggerMessage(
        EventName = "OdesliApiService.ShareLinks.Start",
        Level = LogLevel.Information,
        Message = "Getting share links for '{shareUrl}' on the Odesli API."
    )]
    public static partial void LogOdesliApiServiceShareLinksStart(this ILogger logger, string shareUrl);

    /// <summary>
    /// Logs a failed search on the Odesli API.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="exception">The exception that occurred.</param>
    [LoggerMessage(
        EventName = "OdesliApiService.Search.Failure",
        Level = LogLevel.Error,
        Message = "An error occurred while making a request to the Odesli API."
    )]
    public static partial void LogOdesliApiServiceFailure(this ILogger logger, Exception exception);
}
