namespace MuzakBot.App.Services;

/// <summary>
/// Interface for album release reminder monitor services.
/// </summary>
public interface IAlbumReleaseReminderMonitorService
{
    /// <summary>
    /// Starts the monitor.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StartMonitorAsync(CancellationToken cancellationToken);
}