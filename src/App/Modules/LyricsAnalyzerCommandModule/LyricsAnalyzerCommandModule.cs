using Discord;
using Discord.Interactions;
using MuzakBot.App.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using System.Diagnostics;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule : InteractionModuleBase, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.LyricsAnalyzerCommandModule");
    private readonly IMusicBrainzService _musicBrainzService;
    private readonly IGeniusApiService _geniusApiService;
    private readonly IOpenAiService _openAiService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LyricsAnalyzerCommandModule> _logger;

    public LyricsAnalyzerCommandModule(IMusicBrainzService musicBrainzService, IGeniusApiService geniusApiService, IOpenAiService openAiService, IHttpClientFactory httpClientFactory, ILogger<LyricsAnalyzerCommandModule> logger)
    {
        _musicBrainzService = musicBrainzService;
        _geniusApiService = geniusApiService;
        _openAiService = openAiService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(LyricsAnalyzerCommandModule));

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}