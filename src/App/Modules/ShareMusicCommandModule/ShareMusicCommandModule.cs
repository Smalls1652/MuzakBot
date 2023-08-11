using Discord;
using Discord.Interactions;
using MuzakBot.App.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for housing the music sharing commands.
/// </summary>
public partial class ShareMusicCommandModule : InteractionModuleBase
{
    private readonly IDiscordService _discordService;
    private readonly IOdesliService _odesliService;
    private readonly IItunesApiService _itunesApiService;
    private readonly IMusicBrainzService _musicBrainzService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ShareMusicCommandModule> _logger;

    public ShareMusicCommandModule(IDiscordService discordService, IOdesliService odesliService, IItunesApiService itunesApiService, IMusicBrainzService musicBrainzService, IHttpClientFactory httpClientFactory, ILogger<ShareMusicCommandModule> logger)
    {
        _discordService = discordService;
        _odesliService = odesliService;
        _itunesApiService = itunesApiService;
        _musicBrainzService = musicBrainzService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
}