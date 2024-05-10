using System.Diagnostics;
using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using MuzakBot.Database;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for 'albumrelease' commands.
/// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseCommandModule"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="appleMusicApiService">The Apple Music API service.</param>
    /// <param name="odesliService">The Odesli service.</param>
    /// <param name="albumReleaseDbContextFactory">The <see cref="IDbContextFactory{TContext}"/> for the <see cref="AlbumReleaseDbContext"/>.</param>
    public AlbumReleaseCommandModule(ILogger<AlbumReleaseCommandModule> logger, IAppleMusicApiService appleMusicApiService, IOdesliService odesliService, IDbContextFactory<AlbumReleaseDbContext> albumReleaseDbContextFactory)
    {
        _logger = logger;
        _appleMusicApiService = appleMusicApiService;
        _odesliService = odesliService;
        _albumReleaseDbContextFactory = albumReleaseDbContextFactory;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}