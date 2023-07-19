using Discord;
using Discord.Interactions;
using MuzakBot.App.Services;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule : InteractionModuleBase
{
    private readonly IDiscordService _discordService;
    private readonly IOdesliService _odesliService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ShareMusicCommandModule> _logger;

    public ShareMusicCommandModule(IDiscordService discordService, IOdesliService odesliService, IHttpClientFactory httpClientFactory, ILogger<ShareMusicCommandModule> logger)
    {
        _discordService = discordService;
        _odesliService = odesliService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
}