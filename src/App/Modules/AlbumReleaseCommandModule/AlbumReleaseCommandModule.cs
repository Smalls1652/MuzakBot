using System.Diagnostics;
using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using MuzakBot.Database;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Modules;

[CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
[IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
[Group("albumrelease", "Look up the release date of an album.")]
public partial class AlbumReleaseCommandModule : InteractionModuleBase<SocketInteractionContext>, IDisposable
{
    private bool _isDisposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.AlbumReleaseLookupCommandModule");

    private readonly ILogger _logger;
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly IOdesliService _odesliService;
    private readonly IDbContextFactory<AlbumReleaseDbContext> _albumReleaseDbContextFactory;

    public AlbumReleaseCommandModule(ILogger<AlbumReleaseCommandModule> logger, IAppleMusicApiService appleMusicApiService, IOdesliService odesliService, IDbContextFactory<AlbumReleaseDbContext> albumReleaseDbContextFactory)
    {
        _logger = logger;
        _appleMusicApiService = appleMusicApiService;
        _odesliService = odesliService;
        _albumReleaseDbContextFactory = albumReleaseDbContextFactory;
    }


    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}