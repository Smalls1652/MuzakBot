using System.Diagnostics;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for hosting the lyrics analyzer commands.
/// </summary>
[CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
[IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
public partial class LyricsAnalyzerCommandModule : InteractionModuleBase<SocketInteractionContext>, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.LyricsAnalyzerCommandModule");
    private readonly IMusicBrainzService _musicBrainzService;
    private readonly IGeniusApiService _geniusApiService;
    private readonly IOpenAiService _openAiService;
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LyricsAnalyzerCommandModule> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerCommandModule"/> class.
    /// </summary>
    /// <param name="musicBrainzService">The <see cref="IMusicBrainzService"/>.</param>
    /// <param name="geniusApiService">The <see cref="IGeniusApiService"/>.</param>
    /// <param name="openAiService">The <see cref="IOpenAiService"/>.</param>
    /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
    /// <param name="logger">The logger.</param>
    public LyricsAnalyzerCommandModule(IMusicBrainzService musicBrainzService, IGeniusApiService geniusApiService, IOpenAiService openAiService, ICosmosDbService cosmosDbService, IHttpClientFactory httpClientFactory, ILogger<LyricsAnalyzerCommandModule> logger)
    {
        _musicBrainzService = musicBrainzService;
        _geniusApiService = geniusApiService;
        _openAiService = openAiService;
        _cosmosDbService = cosmosDbService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(LyricsAnalyzerCommandModule));

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}