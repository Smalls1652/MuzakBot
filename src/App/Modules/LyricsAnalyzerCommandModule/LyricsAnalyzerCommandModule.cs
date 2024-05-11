using System.Diagnostics;

using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Metrics;
using MuzakBot.Database;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for hosting the lyrics analyzer commands.
/// </summary>
[CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
[IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
[Group("lyricsanalyzer", "Use AI to analyze the lyrics of a song.")]
public partial class LyricsAnalyzerCommandModule : InteractionModuleBase<SocketInteractionContext>, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.LyricsAnalyzerCommandModule");
    private readonly IOdesliService _odesliService;
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly IGeniusApiService _geniusApiService;
    private readonly IOpenAiService _openAiService;
    private readonly IDbContextFactory<LyricsAnalyzerDbContext> _lyricsAnalyzerDbContextFactory;
    private readonly IQueueClientService _queueClientService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LyricsAnalyzerCommandModule> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerCommandModule"/> class.
    /// </summary>
    /// <param name="odesliService">The <see cref="IOdesliService"/>.</param>
    /// <param name="geniusApiService">The <see cref="IGeniusApiService"/>.</param>
    /// <param name="openAiService">The <see cref="IOpenAiService"/>.</param>
    /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
    /// <param name="logger">The logger.</param>
    public LyricsAnalyzerCommandModule(IOdesliService odesliService, IAppleMusicApiService appleMusicApiService, IGeniusApiService geniusApiService, IOpenAiService openAiService, IDbContextFactory<LyricsAnalyzerDbContext> lyricsAnalyzerContextFactory, IQueueClientService queueClientService, IHttpClientFactory httpClientFactory, ILogger<LyricsAnalyzerCommandModule> logger)
    {
        _odesliService = odesliService;
        _appleMusicApiService = appleMusicApiService;
        _geniusApiService = geniusApiService;
        _openAiService = openAiService;
        _lyricsAnalyzerDbContextFactory = lyricsAnalyzerContextFactory;
        _queueClientService = queueClientService;
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
