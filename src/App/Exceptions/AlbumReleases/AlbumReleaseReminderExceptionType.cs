namespace MuzakBot.App;

/// <summary>
/// The type of exception that occurred while processing an album release reminder.
/// </summary>
public enum AlbumReleaseReminderExceptionType
{
    /// <summary>
    /// An unknown exception occurred.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The Odesli service returned null.
    /// </summary>
    OdesliServiceReturnedNull = 1,

    /// <summary>
    /// The channel specified in the album release reminder does not exist.
    /// </summary>
    ChannelDoesNotExist = 2,

    /// <summary>
    /// The album release reminder failed to send.
    /// </summary>
    FailedToSendMessage = 3
}
