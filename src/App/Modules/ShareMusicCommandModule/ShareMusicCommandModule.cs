using System.Diagnostics;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Metrics;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for housing the music sharing commands.
/// </summary>
[CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
[IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
[Group("sharemusic", "Create share links for a song or album on various streaming platforms.")]
public partial class ShareMusicCommandModule : InteractionModuleBase<SocketInteractionContext>, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.ShareMusicCommandModule");
    private readonly IOdesliService _odesliService;
    private readonly IItunesApiService _itunesApiService;
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly IMusicBrainzService _musicBrainzService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ShareMusicCommandModule> _logger;
    private readonly CommandMetrics _commandMetrics;

    public ShareMusicCommandModule(IOdesliService odesliService, IItunesApiService itunesApiService, IAppleMusicApiService appleMusicApiService, IMusicBrainzService musicBrainzService, IHttpClientFactory httpClientFactory, ILogger<ShareMusicCommandModule> logger, CommandMetrics commandMetrics)
    {
        _odesliService = odesliService;
        _itunesApiService = itunesApiService;
        _appleMusicApiService = appleMusicApiService;
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