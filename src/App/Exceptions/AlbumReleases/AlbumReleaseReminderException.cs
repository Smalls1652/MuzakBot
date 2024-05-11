namespace MuzakBot.App;

/// <summary>
/// An exception that occurred while processing an album release reminder.
/// </summary>
public class AlbumReleaseReminderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseReminderException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="exceptionType">The type of exception that occurred.</param>
    public AlbumReleaseReminderException(string message, AlbumReleaseReminderExceptionType exceptionType) : base(message)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseReminderException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="exceptionType">The type of exception that occurred.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AlbumReleaseReminderException(string message, AlbumReleaseReminderExceptionType exceptionType, Exception innerException) : base(message, innerException)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// The type of exception that occurred.
    /// </summary>
    public AlbumReleaseReminderExceptionType ExceptionType { get; set; }
}
