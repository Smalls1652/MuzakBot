namespace MuzakBot.App;

/// <summary>
/// Represents an exception that occurred during a LyricsAnalyzer database operation.
/// </summary>
public class LyricsAnalyzerDbException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="LyricsAnalyzerDbException"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LyricsAnalyzerDbException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LyricsAnalyzerDbException"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public LyricsAnalyzerDbException(string message, Exception innerException) : base(message, innerException)
    {
    }
}