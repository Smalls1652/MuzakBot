using Azure.Storage.Queues;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Interface for a service that provides a <see cref="Azure.Storage.Queues.QueueClient"/>.
/// </summary>
public interface IQueueClientService
{
    /// <summary>
    /// The Azure Queue Client for the service.
    /// </summary>
    QueueClient QueueClient { get; }
}