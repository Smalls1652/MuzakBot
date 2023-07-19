using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Modules;

namespace MuzakBot.App.Services;

public class DiscordService : IDiscordService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ILogger<DiscordService> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;


    public DiscordService(DiscordSocketClient discordSocketClient, ILogger<DiscordService> logger, IConfiguration config, IServiceProvider serviceProvider)
    {
        _discordSocketClient = discordSocketClient;
        _logger = logger;
        _config = config;
        _serviceProvider = serviceProvider;
    }

    private InteractionService? _interactionService;

    public async Task ConnectAsync()
    {
        // Log into Discord
        _logger.LogInformation("Connecting to Discord...");

        if (_config.GetValue<string>("DiscordClientToken") is null)
        {
            Exception errorException = new("DiscordClientToken is null. Please set the DiscordClientToken environment variable.");

            _logger.LogError(errorException, "{ErrorMessage}", errorException.Message);
            throw errorException;
        }

        await _discordSocketClient.LoginAsync(
            tokenType: TokenType.Bot,
            token: _config.GetValue<string>("DiscordClientToken")
        );

        await _discordSocketClient.StartAsync();

        // Initialize Discord Interaction Service
        _logger.LogInformation("Initializing Discord Interaction Service...");
        _interactionService = new(_discordSocketClient.Rest);

        await _interactionService.AddModuleAsync<ShareMusicCommandModule>(_serviceProvider);

        // Add logging to the DiscordSocketClient and InteractionService
        _discordSocketClient.Log += HandleLog;
        _interactionService.Log += HandleLog;

        // Add slash command handler
        _discordSocketClient.InteractionCreated += HandleSlashCommand;

        // Add ready handler
        _discordSocketClient.Ready += OnClientReadyAsync;
    }

    private async Task OnClientReadyAsync()
    {
#if DEBUG
        ulong testGuildId = _config.GetValue<ulong>("DiscordTestGuildId");
        _logger.LogInformation("Running in debug mode. Registering slash commands to test guild '{GuildId}'.", testGuildId);
        await _interactionService!.RegisterCommandsToGuildAsync(
            guildId: testGuildId,
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

    private async Task HandleSlashCommand(SocketInteraction interaction)
    {
        SocketInteractionContext interactionContext = new(_discordSocketClient, interaction);
        await _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);
    }

    private Task HandleLog(LogMessage logMessage)
    {
        _logger.LogInformation("[{Severity}] {LogMessage}", logMessage.Severity, logMessage.ToString());

        return Task.CompletedTask;
    }

}