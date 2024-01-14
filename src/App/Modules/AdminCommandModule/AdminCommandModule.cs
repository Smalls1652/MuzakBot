using Discord;
using Discord.Interactions;
using MuzakBot.App.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using System.Diagnostics;
using Discord.Commands;
using Discord.WebSocket;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for managing the bot.
/// </summary>
public partial class AdminCommandModule : InteractionModuleBase<SocketInteractionContext>, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.AdminCommandModule");
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AdminCommandModule> _logger;
    private readonly DiscordSocketClient _discordSocketClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerCommandModule"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
    /// <param name="logger">The logger.</param>
    public AdminCommandModule(ICosmosDbService cosmosDbService, IHttpClientFactory httpClientFactory, ILogger<AdminCommandModule> logger, DiscordSocketClient discordSocketClient)
    {
        _cosmosDbService = cosmosDbService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _discordSocketClient = discordSocketClient;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(AdminCommandModule));

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}