namespace MuzakBot.Lib;

/// <summary>
/// Represents an that occurred during a Genius API request.
/// </summary>
public class GeniusApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="GeniusApiException"/>.
    /// </summary>
    /// <param name="exceptionType">The type of exception that occurred.</param>
    /// <param name="message">The message that describes the error.</param>
    public GeniusApiException(GeniusApiExceptionType exceptionType, string message) : base(message)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GeniusApiException"/>.
    /// </summary>
    /// <param name="exceptionType">The type of exception that occurred.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public GeniusApiException(GeniusApiExceptionType exceptionType, string message, Exception innerException) : base(message, innerException)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// The type of exception that occurred during the Genius API request.
    /// </summary>
    public GeniusApiExceptionType ExceptionType { get; }
}
