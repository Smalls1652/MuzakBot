namespace MuzakBot.GeniusService.Services;

/// <summary>
/// Interface for services that monitor Azure Queues.
/// </summary>
public interface IAzureQueueMonitorService
{
    /// <summary>
    /// Starts monitoring the Azure Queue.
    /// </summary>
    void StartMonitor();
}