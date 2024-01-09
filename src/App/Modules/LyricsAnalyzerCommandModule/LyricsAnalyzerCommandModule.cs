using Discord;
using Discord.Interactions;
using MuzakBot.App.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using System.Diagnostics;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for hosting the lyrics analyzer commands.
/// </summary>
public partial class LyricsAnalyzerCommandModule : InteractionModuleBase, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.LyricsAnalyzerCommandModule");
    private readonly IMusicBrainzService _musicBrainzService;
    private readonly IGeniusApiService _geniusApiService;
    private readonly IOpenAiService _openAiService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LyricsAnalyzerCommandModule> _logger;
    private readonly DiscordServiceConfig _discordServiceConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerCommandModule"/> class.
    /// </summary>
    /// <param name="musicBrainzService">The <see cref="IMusicBrainzService"/>.</param>
    /// <param name="geniusApiService">The <see cref="IGeniusApiService"/>.</param>
    /// <param name="openAiService">The <see cref="IOpenAiService"/>.</param>
    /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
    /// <param name="logger">The logger.</param>
    public LyricsAnalyzerCommandModule(IMusicBrainzService musicBrainzService, IGeniusApiService geniusApiService, IOpenAiService openAiService, IHttpClientFactory httpClientFactory, ILogger<LyricsAnalyzerCommandModule> logger, DiscordServiceConfig discordServiceConfig)
    {
        _musicBrainzService = musicBrainzService;
        _geniusApiService = geniusApiService;
        _openAiService = openAiService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _discordServiceConfig = discordServiceConfig;
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