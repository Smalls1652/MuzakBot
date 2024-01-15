using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Logging.OpenAi;

/// <summary>
/// Source generated logging methods for the <see cref="Services.OpenAiService"/> class.
/// </summary>
internal static partial class OpenAiServiceLogging
{
    /// <summary>
    /// Logs the start of a lyric analysis.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="promptMode">The prompt mode being used.</param>
    [LoggerMessage(
        EventName = "OpenAiApiService.LyricAnalysis.Start",
        Level = LogLevel.Information,
        Message = "Starting lyric analysis for '{artistName} - {songName}' using the '{promptMode}' style."
    )]
    public static partial void LogOpenAiApiServiceLyricAnalysisStart(this ILogger logger, string artistName, string songName, string promptMode);

    /// <summary>
    /// Logs a failed lyric analysis.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="exception">The exception that occurred.</param>
    [LoggerMessage(
        EventName = "OpenAiApiService.LyricAnalysis.Failure",
        Level = LogLevel.Error,
        Message = "An error occurred while making a request to the OpenAI API."
    )]
    public static partial void LogOpenAiApiServiceFailure(this ILogger logger, Exception exception);
}