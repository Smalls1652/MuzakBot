using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MuzakBot.GeniusService.TaskQueues;
using MuzakBot.Lib.Services;

namespace MuzakBot.GeniusService.Services;

public sealed class MainService : IHostedService, IDisposable
{
    private bool _disposed;
    private Task? _task;
    private Task? _monitorTask;
    private CancellationTokenSource? _cts;

    private readonly IAzureQueueMonitorService _queueMonitorService;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly MainServiceOptions _options;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public MainService(IAzureQueueMonitorService queueMonitorService, IBackgroundTaskQueue taskQueue, IOptions<MainServiceOptions> options, ILogger<MainService> logger, IHostApplicationLifetime appLifetime)
    {
        _queueMonitorService = queueMonitorService;
        _taskQueue = taskQueue;
        _options = options.Value;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting the Azure Queue Task Processor...");
        return ProcessAzureTaskQueueAsync(cancellationToken);
    }

    private async Task ProcessAzureTaskQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask>? workItem = await _taskQueue.DequeueAsync(cancellationToken);

                await workItem(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Do nothing.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the Azure Task Queue.");
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _task = RunAsync(_cts.Token);
        _monitorTask = Task.Run(() => _queueMonitorService.StartMonitor(), _cts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_task is not null)
        {
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                await _task
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
        }

        if (_monitorTask is not null)
        {
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                await _monitorTask
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
        }
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _disposed = true;

        _cts?.Dispose();
        _task?.Dispose();
        _monitorTask?.Dispose();

        GC.SuppressFinalize(this);
    }
}