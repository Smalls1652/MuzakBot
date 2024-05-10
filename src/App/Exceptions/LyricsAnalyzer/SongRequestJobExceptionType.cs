namespace MuzakBot.App;

/// <summary>
/// Represents the type of exception that occurred during a song request job.
/// </summary>
public enum SongRequestJobExceptionType
{
    /// <summary>
    /// An unknown exception occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The standalone service did not acknowledge the request.
    /// </summary>
    StandaloneServiceNotAcknowledged = 1,

    /// <summary>
    /// The standalone service indicated that the fallback method is needed.
    /// </summary>
    FallbackMethodNeeded = 2,

    /// <summary>
    /// The song request job timed out.
    /// </summary>
    Timeout = 3,

    /// <summary>
    /// The lyrics returned were null.
    /// </summary>
    LyricsReturnedNull = 4,

    /// <summary>
    /// The fallback job failed to retrieve lyrics.
    /// </summary>
    FallbackJobFailed = 5
}