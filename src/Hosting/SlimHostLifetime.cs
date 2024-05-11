using System.Runtime.InteropServices;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MuzakBot.Hosting;

/// <summary>
/// A custom host lifetime implementation that keeps lifecycle logging to a minimum.
/// </summary>
public sealed class SlimHostLifetime : IHostLifetime, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SlimHostLifetime"/> class.
    /// </summary>
    /// <param name="environment">The <see cref="IHostEnvironment"/>.</param>
    /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public SlimHostLifetime(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
    {
        Environment = environment;
        ApplicationLifetime = applicationLifetime;
        Logger = loggerFactory.CreateLogger("MuzakBot.Hosting");
    }

    private IHostEnvironment Environment { get; }
    private IHostApplicationLifetime ApplicationLifetime { get; }
    private ILogger Logger { get; }

    private PosixSignalRegistration? _sigIntRegistration;
    private PosixSignalRegistration? _sigQuitRegistration;
    private PosixSignalRegistration? _sigTermRegistration;
    private CancellationTokenRegistration _appStoppingRegistration;

    /// <inheritdoc />
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        _appStoppingRegistration = ApplicationLifetime.ApplicationStopping.Register(state =>
        {
            ((SlimHostLifetime)state!).OnAppStopping();
        }, this);

        RegisterShutdownHandlers();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        UnregisterShutdownHandlers();
        _appStoppingRegistration.Dispose();
    }

    /// <summary>
    /// Called when the application is stopping and logs an
    /// application shutdown message.
    /// </summary>
    private void OnAppStopping()
    {
        Logger.LogInformation("Application is shutting down...");
    }

    /// <summary>
    /// Registers the shutdown handlers for SIGINT, SIGQUIT, and SIGTERM signals.
    /// </summary>
    private void RegisterShutdownHandlers()
    {
        Action<PosixSignalContext> handler = HandlePosixSignal;
        _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
        _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);
    }

    /// <summary>
    /// Handles the SIGINT, SIGQUIT, and SIGTERM signals by stopping the application.
    /// </summary>
    /// <param name="context">The <see cref="PosixSignalContext"/>.</param>
    private void HandlePosixSignal(PosixSignalContext context)
    {
        context.Cancel = true;
        ApplicationLifetime.StopApplication();
    }

    /// <summary>
    /// Unregisters the shutdown handlers for SIGINT, SIGQUIT, and SIGTERM signals.
    /// </summary>
    private void UnregisterShutdownHandlers()
    {
        _sigIntRegistration?.Dispose();
        _sigQuitRegistration?.Dispose();
        _sigTermRegistration?.Dispose();
    }
}
