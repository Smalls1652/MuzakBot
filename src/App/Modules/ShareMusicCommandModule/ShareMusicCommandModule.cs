using Discord;
using Discord.Interactions;
using MuzakBot.Lib.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using System.Diagnostics;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for housing the music sharing commands.
/// </summary>
public partial class ShareMusicCommandModule : InteractionModuleBase, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.ShareMusicCommandModule");
    private readonly IOdesliService _odesliService;
    private readonly IItunesApiService _itunesApiService;
    private readonly IMusicBrainzService _musicBrainzService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ShareMusicCommandModule> _logger;
    private readonly CommandMetrics _commandMetrics;

    public ShareMusicCommandModule(IOdesliService odesliService, IItunesApiService itunesApiService, IMusicBrainzService musicBrainzService, IHttpClientFactory httpClientFactory, ILogger<ShareMusicCommandModule> logger, CommandMetrics commandMetrics)
    {
        _odesliService = odesliService;
        _itunesApiService = itunesApiService;
        _musicBrainzService = musicBrainzService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _commandMetrics = commandMetrics;
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, nameof(ShareMusicCommandModule));

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}