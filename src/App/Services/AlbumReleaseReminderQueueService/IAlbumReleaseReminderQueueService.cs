namespace MuzakBot.App.Services;

public interface IAlbumReleaseReminderQueueService
{
    Task StartMonitorAsync(CancellationToken cancellationToken);
}
