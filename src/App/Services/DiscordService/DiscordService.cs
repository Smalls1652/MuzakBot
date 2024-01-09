using System.Diagnostics;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuzakBot.App.Modules;

namespace MuzakBot.App.Services;

/// <summary>
/// The service for interacting with Discord.
/// </summary>
public class DiscordService : IDiscordService, IHostedService
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.DiscordService");
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<DiscordService> _logger;
    private readonly string _clientToken;
#if DEBUG
    private readonly ulong _testGuildId;
#endif
    private readonly bool _enableLyricsAnalyzer;
    private readonly IServiceProvider _serviceProvider;

    public DiscordService(DiscordSocketClient discordSocketClient, ILogger<DiscordService> logger, IOptions<DiscordServiceOptions> options, IServiceProvider serviceProvider)
    {
        _discordSocketClient = discordSocketClient;
        _logger = logger;
        _clientToken = options.Value.ClientToken ?? throw new ArgumentNullException(nameof(options), "Client token is null.");
#if DEBUG
        _testGuildId = ulong.Parse(options.Value.TestGuildId ?? throw new ArgumentNullException(nameof(options), "Test guild ID is null."));
#endif
        _enableLyricsAnalyzer = options.Value.EnableLyricsAnalyzer;
        _serviceProvider = serviceProvider;
    }

    private InteractionService? _interactionService;

    /// <inheritdoc cref="IDiscordService.ConnectAsync"/>
    public async Task ConnectAsync()
    {
        // Log into Discord
        _logger.LogInformation("Connecting to Discord...");

        if (_clientToken is null)
        {
            Exception errorException = new("DISCORD_CLIENT_TOKEN is null. Please set the DISCORD_CLIENT_TOKEN environment variable.");

            _logger.LogError(errorException, "{ErrorMessage}", errorException.Message);
            throw errorException;
        }

        await _discordSocketClient.LoginAsync(
            tokenType: TokenType.Bot,
            token: _clientToken
        );

        await _discordSocketClient.StartAsync();

        // Initialize Discord Interaction Service
        _logger.LogInformation("Initializing Discord Interaction Service...");
        _interactionService = new(_discordSocketClient.Rest);

        _logger.LogInformation("Adding 'ShareMusicCommandModule'.");
        await _interactionService.AddModuleAsync<ShareMusicCommandModule>(_serviceProvider);

        if (_enableLyricsAnalyzer)
        {
            _logger.LogInformation("Lyrics analyzer is enabled. Adding 'LyricsAnalyzerCommandModule'.");
            await _interactionService.AddModuleAsync<LyricsAnalyzerCommandModule>(_serviceProvider);
        }
        else
        {
            _logger.LogInformation("Lyrics analyzer is disabled.");
        }

        // Add logging to the DiscordSocketClient and InteractionService
        _discordSocketClient.Log += HandleLog;

        // Add slash command handler
        _discordSocketClient.InteractionCreated += HandleSlashCommand;

        // Add ready handler
        _discordSocketClient.Ready += OnClientReadyAsync;
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
        _logger.LogInformation("Running in debug mode. Registering slash commands to test guild '{GuildId}'.", _testGuildId);
        await _interactionService!.RegisterCommandsToGuildAsync(
            guildId: _testGuildId,
            deleteMissing: true
        );
#else
        _logger.LogInformation("Registering slash commands globally.");
        await _interactionService!.RegisterCommandsGloballyAsync(
            deleteMissing: true
        );
#endif

        string slashCommandsLoadedString = string.Join(",", _interactionService.SlashCommands);
        _logger.LogInformation("Slash commands loaded: {SlashCommandsLoaded}", slashCommandsLoadedString);
    }

    /// <summary>
    /// Handler for the interaction service when a slash command is received.
    /// </summary>
    /// <param name="interaction">Interaction received from Discord's WebSocket API.</param>
    private async Task HandleSlashCommand(SocketInteraction interaction)
    {
        SocketInteractionContext interactionContext = new(_discordSocketClient, interaction);
        await _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disconnecting from Discord...");

        await _discordSocketClient.LogoutAsync();

        _logger.LogInformation("Disconnected.");
    }

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

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}