namespace MuzakBot.App;

/// <summary>
/// Represents an exception that occurred during a song request job.
/// </summary>
public class SongRequestJobException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="SongRequestJobException"/>.
    /// </summary>
    /// <param name="exceptionType">The type of exception that occurred.</param>
    /// <param name="message">The message that describes the error.</param>
    public SongRequestJobException(SongRequestJobExceptionType exceptionType, string message)
        : base(message)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SongRequestJobException"/>.
    /// </summary>
    /// <param name="exceptionType">The type of exception that occurred.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SongRequestJobException(SongRequestJobExceptionType exceptionType, string message, Exception innerException)
        : base(message, innerException)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// The type of exception that occurred during the song request job.
    /// </summary>
    public SongRequestJobExceptionType ExceptionType { get; }
}