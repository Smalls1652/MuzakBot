using System.Text;
using System.Text.Json;

using Azure;
using Azure.Storage.Queues.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.Database;
using MuzakBot.GeniusService.TaskQueues;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.QueueMessages;
using MuzakBot.Lib.Services;

namespace MuzakBot.GeniusService.Services;

/// <summary>
/// Service that monitors an Azure Queue for messages.
/// </summary>
public sealed class AzureQueueMonitorService : IAzureQueueMonitorService
{
    private readonly IGeniusApiService _geniusApiService;
    private readonly IQueueClientService _queueClientService;
    private readonly IDbContextFactory<LyricsAnalyzerDbContext> _lyricsAnalyzerDbContextFactory;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger _logger;
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureQueueMonitorService"/> class.
    /// </summary>
    /// <param name="geniusApiService">The <see cref="IGeniusApiService"/>.</param>
    /// <param name="queueClientService">The <see cref="IQueueClientService"/>.</param>
    /// <param name="taskQueue">The <see cref="IBackgroundTaskQueue"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="appLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    public AzureQueueMonitorService(IGeniusApiService geniusApiService, IQueueClientService queueClientService, IDbContextFactory<LyricsAnalyzerDbContext> lyricsAnalyzerDbContextFactory, IBackgroundTaskQueue taskQueue, ILogger<AzureQueueMonitorService> logger, IHostApplicationLifetime appLifetime)
    {
        _geniusApiService = geniusApiService;
        _queueClientService = queueClientService;
        _lyricsAnalyzerDbContextFactory = lyricsAnalyzerDbContextFactory;
        _taskQueue = taskQueue;
        _logger = logger;

        _cancellationToken = appLifetime.ApplicationStopping;
    }

    /// <inheritdoc />
    public void StartMonitor()
    {
        Task.Run(async () => await MonitorAzureMonitorQueueAsync());
    }

    /// <summary>
    /// Monitors the Azure Queue for messages.
    /// </summary>
    /// <returns></returns>
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

            await Task.Delay(500, _cancellationToken);
        }
    }

    /// <summary>
    /// Processes a queue message.
    /// </summary>
    /// <param name="message">The <see cref="QueueMessage"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns></returns>
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

        await using LyricsAnalyzerDbContext dbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext();

        SongLyricsRequestJob requestJobInDb = await dbContext.SongLyricsRequestJobs.FirstAsync(item => item.Id == requestMessage.JobId);

        requestJobInDb.StandaloneServiceAcknowledged = true;

        _logger.LogInformation("Acknowledging job ({JobId}).", requestMessage.JobId);
        dbContext.SongLyricsRequestJobs.Update(requestJobInDb);
        await dbContext.SaveChangesAsync(cancellationToken);

        string? lyrics = null;
        try
        {
            lyrics = await _geniusApiService.GetLyricsDirectlyAsync(requestMessage.GeniusUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching lyrics from the Genius API.");

            requestJobInDb.FallbackMethodNeeded = true;
            dbContext.SongLyricsRequestJobs.Update(requestJobInDb);

            await dbContext.SaveChangesAsync(cancellationToken);

            await _queueClientService.QueueClient.DeleteMessageAsync(
                messageId: message.MessageId,
                popReceipt: message.PopReceipt,
                cancellationToken: cancellationToken
            );

            return;
        }

        SongLyricsItem songLyricsItem = new(
            artistName: requestMessage.ArtistName,
            songName: requestMessage.SongTitle,
            lyrics: lyrics
        );

        await dbContext.SongLyricsItems.AddAsync(songLyricsItem, cancellationToken);

        requestJobInDb.IsCompleted = true;
        requestJobInDb.SongLyricsItemId = songLyricsItem.Id;

        dbContext.SongLyricsRequestJobs.Update(requestJobInDb);

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully processed queue message ({MessageId}).", message.MessageId);

        await _queueClientService.QueueClient.DeleteMessageAsync(
            messageId: message.MessageId,
            popReceipt: message.PopReceipt,
            cancellationToken: cancellationToken
        );
    }
}