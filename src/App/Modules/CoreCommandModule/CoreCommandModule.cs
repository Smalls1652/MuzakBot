using System.Diagnostics;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

/// <summary>
/// Command module for hosting core/miscellaneous commands.
/// </summary>
[CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
[IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
public partial class CoreCommandModule : InteractionModuleBase<SocketInteractionContext>, IDisposable
{
    private bool _disposed;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Modules.CoreCommandModule");
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreCommandModule"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger.</param>
    public CoreCommandModule(
        DiscordSocketClient discordSocketClient,
        IConfiguration configuration,
        ILogger<CoreCommandModule> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _activitySource.Dispose();

        _disposed = true;

        GC.SuppressFinalize(this);
    }
}