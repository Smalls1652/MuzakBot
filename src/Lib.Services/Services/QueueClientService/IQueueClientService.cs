using Azure.Storage.Queues;

namespace MuzakBot.Lib.Services;

public interface IQueueClientService
{
    QueueClient QueueClient { get; }
}
