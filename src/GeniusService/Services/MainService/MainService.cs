using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MuzakBot.Lib.Services;

namespace MuzakBot.GeniusService.Services;

public sealed class MainService : IHostedService, IDisposable
{
    private bool _disposed;
    private Task? _task;
    private CancellationTokenSource? _cts;

    private readonly IQueueClientService _queueClientService;
    private readonly MainServiceOptions _options;
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public MainService(IQueueClientService queueClientService, IOptions<MainServiceOptions> options, ILogger<MainService> logger, IHostApplicationLifetime appLifetime)
    {
        _queueClientService = queueClientService;
        _options = options.Value;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Running...");

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
        finally
        {
            _appLifetime.StopApplication();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _task = RunAsync(_cts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_task is null)
        {
            return;
        }

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

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _disposed = true;

        _cts?.Dispose();
        _task?.Dispose();

        GC.SuppressFinalize(this);
    }
}