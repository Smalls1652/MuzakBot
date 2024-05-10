using System.Diagnostics;
using System.Reflection;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MuzakBot.App.Logging.Discord;
using MuzakBot.App.Modules;

namespace MuzakBot.App.Services;

/// <summary>
/// The service for interacting with Discord.
/// </summary>
public class DiscordService : IDiscordService, IHostedService
{
    private bool _isDisposed;
    private CancellationTokenSource? _cts;
    private Task? _runTask;
    private Task? _albumReleaseReminderMonitorTask;

    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.DiscordService");
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly ILogger<DiscordService> _logger;
    private readonly IAlbumReleaseReminderMonitorService _albumReleaseReminderQueueService;
    private readonly string _clientToken;
#if DEBUG
    private readonly ulong _testGuildId;
#endif
    private readonly ulong _adminGuildId;
    private readonly IServiceProvider _serviceProvider;

    public DiscordService(DiscordSocketClient discordSocketClient, InteractionService interactionService, ILogger<DiscordService> logger, IAlbumReleaseReminderMonitorService albumReleaseReminderQueueService, IOptions<DiscordServiceOptions> options, IServiceProvider serviceProvider)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _logger = logger;
        _albumReleaseReminderQueueService = albumReleaseReminderQueueService;
        _clientToken = options.Value.ClientToken ?? throw new ArgumentNullException(nameof(options), "Client token is null.");
#if DEBUG
        _testGuildId = ulong.Parse(options.Value.TestGuildId ?? throw new ArgumentNullException(nameof(options), "Test guild ID is null."));
#endif
        _adminGuildId = options.Value.AdminGuildId;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc cref="IDiscordService.ConnectAsync"/>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        // Log into Discord
        _logger.LogDiscordStartConnecting();

        if (_clientToken is null)
        {
            Exception errorException = new("DISCORD_CLIENT_TOKEN is null. Please set the DISCORD_CLIENT_TOKEN environment variable.");

            _logger.LogMissingConfigValueError(errorException.Message, errorException);
            throw errorException;
        }

        // Initialize Discord Interaction Service
        _logger.LogInitializeDiscordInteractionService();

        _logger.LogAddModuleToInteractionService("ShareMusicCommandModule");
        await _interactionService.AddModuleAsync<ShareMusicCommandModule>(_serviceProvider);

        _logger.LogAddModuleToInteractionService("AdminCommandModule");
        await _interactionService.AddModuleAsync<AdminCommandModule>(_serviceProvider);

        _logger.LogAddModuleToInteractionService("LyricsAnalyzerCommandModule");
        await _interactionService.AddModuleAsync<LyricsAnalyzerCommandModule>(_serviceProvider);

        _logger.LogAddModuleToInteractionService("CoreCommandModule");
        await _interactionService.AddModuleAsync<CoreCommandModule>(_serviceProvider);

        _logger.LogAddModuleToInteractionService("AlbumReleaseCommandModule");
        await _interactionService.AddModuleAsync<AlbumReleaseCommandModule>(_serviceProvider);

        // Add logging to the DiscordSocketClient and InteractionService
        _discordSocketClient.Log += HandleLog;
        _interactionService.Log += HandleLog;

        // Add interaction handler
        _discordSocketClient.InteractionCreated += HandleInteraction;

        // Add slash command handler
        //_discordSocketClient.InteractionCreated += HandleSlashCommand;

        // Add autocomplete handler
        //_discordSocketClient.InteractionCreated += HandleAutocomplete;

        // Add ready handler
        _discordSocketClient.Ready += OnClientReadyAsync;

        await _discordSocketClient.LoginAsync(
            tokenType: TokenType.Bot,
            token: _clientToken
        );

        await _discordSocketClient.StartAsync();
    }

    /// <summary>
    /// Called when the Discord client is ready.
    /// <list type="table">
    ///     <listheader>
    ///         <term>Task</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Register commands</term>
    ///         <description>Registers slash commands to the test guild if in debug mode, otherwise registers slash commands globally.</description>
    ///     </item>
    /// </list>
    /// </summary>
    private async Task OnClientReadyAsync()
    {
#if DEBUG
        _logger.LogRegisterCommandsDebugMode(_testGuildId);

        await _interactionService!.RegisterCommandsToGuildAsync(
            guildId: _testGuildId,
            deleteMissing: true
        );
#else
        _logger.LogRegisterCommandsGlobally();
        await _interactionService.AddModulesGloballyAsync(
            deleteMissing: true,
            modules: [
                _interactionService.GetModuleInfo<ShareMusicCommandModule>(),
                _interactionService.GetModuleInfo<LyricsAnalyzerCommandModule>(),
                _interactionService.GetModuleInfo<CoreCommandModule>(),
                _interactionService.GetModuleInfo<AlbumReleaseCommandModule>()
            ]
        );

        _logger.LogRegisterAdminCommands(_adminGuildId);
        await _interactionService.AddModulesToGuildAsync(
            guildId: _adminGuildId,
            deleteMissing: true,
            modules: [
                _interactionService.GetModuleInfo<AdminCommandModule>()
            ]
        );
#endif

        string slashCommandsLoadedString = string.Join(",", _interactionService.SlashCommands);
        _logger.LogSlashCommandsLoaded(slashCommandsLoadedString);

        _albumReleaseReminderMonitorTask = _albumReleaseReminderQueueService.StartMonitorAsync(_cts!.Token);
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_discordSocketClient, interaction);

        var result = await _interactionService!.ExecuteCommandAsync(context, _serviceProvider);

        if (!result.IsSuccess)
        {
            _logger.LogFailedToExecuteInteraction(interaction.User.Username, interaction.IsDMInteraction);
            _logger.LogInteractionErrorMessage(result.ErrorReason);
        }
    }

    /// <summary>
    /// Handler for the interaction service when a slash command is received.
    /// </summary>
    /// <param name="interaction">Interaction received from Discord's WebSocket API.</param>
    private async Task HandleSlashCommand(SocketInteraction interaction)
    {
        SocketInteractionContext interactionContext = new(_discordSocketClient, interaction);

        var result = await _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);

        if (!result.IsSuccess)
        {
            _logger.LogFailedToExecuteSlashCommand(interaction.User.Username, interaction.IsDMInteraction);
            _logger.LogSlashCommandErrorMessage(result.ErrorReason);
        }
    }

    /// <summary>
    /// Handler for the interaction service when an autocomplete is received.
    /// </summary>
    /// <param name="interaction">Interaction received from Discord's WebSocket API.</param>
    /// <returns></returns>
    private async Task HandleAutocomplete(SocketInteraction interaction)
    {
        SocketInteractionContext interactionContext = new(_discordSocketClient, interaction);

        var result = await _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);

        if (!result.IsSuccess)
        {
            _logger.LogFailedToExecuteAutocomplete(interaction.User.Username, interaction.IsDMInteraction);
            _logger.LogAutocompleteErrorMessage(result.ErrorReason);
        }
    }

    /// <summary>
    /// Handler for outputting log messages from <see cref="DiscordSocketClient"/> and <see cref="InteractionService"/>
    /// to the dependency injected logger.
    /// </summary>
    /// <param name="logMessage">The log message generated by the Discord client.</param>
    /// <returns></returns>
    private Task HandleLog(LogMessage logMessage)
    {
        LogLevel logLevel = logMessage.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(logMessage.Severity), logMessage.Severity, "Unknown log severity.")
        };

        string logMessageString;
        if (logMessage.Exception is CommandException cmdException)
        {
            logMessageString = $"{cmdException.Command.Aliases.First()} failed to execute in {cmdException.Context.Channel}.";
        }
        else
        {
            logMessageString = $"{logMessage}";
        }

        _logger.Log(
            logLevel: logLevel,
            message: logMessageString,
            exception: logMessage.Exception
        );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts the Discord service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _runTask = ConnectAsync(_cts.Token);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the Discord service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDiscordBotDisconnect();

        await _discordSocketClient.LogoutAsync();

        _logger.LogDiscordBotDisconnected();

        if (_runTask is not null)
        {
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                await _runTask
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
        }

        if (_albumReleaseReminderMonitorTask is not null)
        {
            try
            {
                _cts?.Cancel();
            }
            finally
            {
                await _albumReleaseReminderMonitorTask
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
        }

    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(DiscordService));

        _discordSocketClient.Log -= HandleLog;
        _discordSocketClient.InteractionCreated -= HandleSlashCommand;
        _discordSocketClient.Ready -= OnClientReadyAsync;

        if (_interactionService is not null)
        {
            _interactionService.Dispose();
        }

        await _discordSocketClient.DisposeAsync();

        _runTask?.Dispose();
        _albumReleaseReminderMonitorTask?.Dispose();

        _activitySource.Dispose();
        _cts?.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}