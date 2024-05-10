namespace MuzakBot.App.Services;

public interface IAlbumReleaseReminderMonitorService
{
    Task StartMonitorAsync(CancellationToken cancellationToken);
}
