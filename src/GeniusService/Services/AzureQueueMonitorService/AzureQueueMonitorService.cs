using System.Text;
using System.Text.Json;

using Azure;
using Azure.Storage.Queues.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.GeniusService.TaskQueues;
using MuzakBot.Lib.Models.QueueMessages;
using MuzakBot.Lib.Services;

namespace MuzakBot.GeniusService.Services;

public sealed class AzureQueueMonitorService : IAzureQueueMonitorService
{
    private IGeniusApiService _geniusApiService;
    private IQueueClientService _queueClientService;
    private IBackgroundTaskQueue _taskQueue;
    private readonly ILogger _logger;

    private readonly CancellationToken _cancellationToken;

    public AzureQueueMonitorService(IGeniusApiService geniusApiService, IQueueClientService queueClientService, IBackgroundTaskQueue taskQueue, ILogger<AzureQueueMonitorService> logger, IHostApplicationLifetime appLifetime)
    {
        _geniusApiService = geniusApiService;
        _queueClientService = queueClientService;
        _taskQueue = taskQueue;
        _logger = logger;

        _cancellationToken = appLifetime.ApplicationStopping;
    }

    public void StartMonitor()
    {
        Task.Run(async () => await MonitorAzureMonitorQueueAsync());
    }

    private async ValueTask MonitorAzureMonitorQueueAsync()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            Response<QueueMessage[]> queueMessages = await _queueClientService.QueueClient.ReceiveMessagesAsync(
                maxMessages: 32,
                cancellationToken: _cancellationToken
            );

            if (queueMessages.Value.Length > 0)
            {
                _logger.LogInformation("Received {Count} messages from the Azure Queue.", queueMessages.Value.Length);

                foreach (QueueMessage message in queueMessages.Value)
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(
                        async (cancellationToken) => await ProcessQueueMessageAsync(message, cancellationToken)
                    );
                }
            }

            await Task.Delay(3000, _cancellationToken);
        }
    }

    private async ValueTask ProcessQueueMessageAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        string messageBody = Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()));

        _logger.LogInformation("Processing queue message ({MessageId}).", message.MessageId);

        SongLyricsRequestMessage requestMessage;
        try
        {
            requestMessage = JsonSerializer.Deserialize(
                json: messageBody,
                jsonTypeInfo: QueueMessageJsonContext.Default.SongLyricsRequestMessage
            ) ?? throw new NullReferenceException("JSON deserialization returned null.");
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "JSON deserialization returned a null value.");
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the queue message.");
            return;
        }

        string? lyrics = null;
        try
        {
            lyrics = await _geniusApiService.GetLyricsDirectlyAsync(requestMessage.GeniusUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching lyrics from the Genius API.");

            if (message.DequeueCount >= 5)
            {
                _logger.LogWarning("Queue message ({MessageId}) has been dequeued 5 times. Deleting message.", message.MessageId);

                await _queueClientService.QueueClient.DeleteMessageAsync(
                    messageId: message.MessageId,
                    popReceipt: message.PopReceipt,
                    cancellationToken: cancellationToken
                );
            }

            return;
        }

        _logger.LogInformation("Successfully processed queue message ({MessageId}).", message.MessageId);

        await _queueClientService.QueueClient.DeleteMessageAsync(
            messageId: message.MessageId,
            popReceipt: message.PopReceipt,
            cancellationToken: cancellationToken
        );

        if (lyrics is not null)
        {
            _logger.LogInformation("{Lyrics}", lyrics);
        }
    }
}
