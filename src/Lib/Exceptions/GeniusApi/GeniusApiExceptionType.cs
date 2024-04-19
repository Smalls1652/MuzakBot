namespace MuzakBot.Lib;

/// <summary>
/// Represents the type of exception that occurred during a Genius API request.
/// </summary>
public enum GeniusApiExceptionType
{
    /// <summary>
    /// An unknown exception occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// No results were found.
    /// </summary>
    NoResults = 1,

    /// <summary>
    /// No songs were found.
    /// </summary>
    NoSongsFound = 2
}
