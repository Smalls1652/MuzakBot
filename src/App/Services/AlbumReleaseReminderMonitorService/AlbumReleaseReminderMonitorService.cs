using System.Diagnostics;
using System.Text;

using Discord;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Models.Responses;
using MuzakBot.App.Services;
using MuzakBot.Database;
using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Database.AlbumRelease;
using MuzakBot.Lib.Models.Odesli;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Services;

/// <summary>
/// Service for monitoring album release reminders and sending them to the appropriate guilds/channels.
/// </summary>
public sealed class AlbumReleaseReminderMonitorService : IAlbumReleaseReminderMonitorService, IDisposable
{
    private bool _isDisposed;

    private readonly ILogger _logger;
    private readonly DiscordSocketClient _discordClient;
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly IOdesliService _odesliService;
    private readonly IDbContextFactory<AlbumReleaseDbContext> _albumReleaseDbContextFactory;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.AlbumReleaseReminderMonitorService");

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseReminderMonitorService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="discordClient">The Discord client.</param>
    /// <param name="appleMusicApiService">The Apple Music API service.</param>
    /// <param name="odesliService">The Odesli service.</param>
    /// <param name="albumReleaseDbContextFactory">The <see cref="IDbContextFactory{TContext}"/> for the <see cref="AlbumReleaseDbContext"/>.</param>
    public AlbumReleaseReminderMonitorService(ILogger<AlbumReleaseReminderMonitorService> logger, DiscordSocketClient discordClient, IAppleMusicApiService appleMusicApiService, IOdesliService odesliService, IDbContextFactory<AlbumReleaseDbContext> albumReleaseDbContextFactory)
    {
        _logger = logger;
        _discordClient = discordClient;
        _appleMusicApiService = appleMusicApiService;
        _odesliService = odesliService;
        _albumReleaseDbContextFactory = albumReleaseDbContextFactory;
    }

    /// <inheritdoc/>
    public Task StartMonitorAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () => await ProcessAlbumReleaseRemindersAsync(cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Processes the album release reminders.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    private async ValueTask ProcessAlbumReleaseRemindersAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting album release reminder queue service.");

        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Processing album release reminders.");

            foreach (SocketGuild guildItem in _discordClient.Guilds)
            {
                AlbumReleaseDbContext dbContext = _albumReleaseDbContextFactory.CreateDbContext();

                try
                {
                    List<AlbumReleaseReminder> albumReleaseReminders = await dbContext.AlbumReleaseReminders
                        .Where(item => item.GuildId == guildItem.Id.ToString() && !item.ReminderSent)
                        .ToListAsync(cancellationToken);

                    if (albumReleaseReminders.Count == 0)
                    {
                        continue;
                    }

                    DateTimeOffset currentTime = DateTimeOffset.UtcNow;

                    foreach (var releaseReminder in albumReleaseReminders)
                    {
                        if (releaseReminder.ReleaseDate <= currentTime)
                        {
                            await SendAlbumReleaseReminderAsync(releaseReminder, guildItem);
                        }
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
        }

        _logger.LogInformation("Stopping album release reminder queue service.");
    }

    /// <summary>
    /// Sends an album release reminder to the specified guild.
    /// </summary>
    /// <param name="releaseReminder">The album release reminder.</param>
    /// <param name="guildItem">The guild.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Thrown when share links from Odesli or the channel are null.</exception>
    /// <exception cref="Exception">Thrown when the album release reminder fails to send.</exception>
    private async Task SendAlbumReleaseReminderAsync(AlbumReleaseReminder releaseReminder, SocketGuild guildItem)
    {
        using var activity = _activitySource.StartActivity(
            name: "SendAlbumReleaseReminderAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "album_Id", releaseReminder.AlbumId },
                { "guild_Id", releaseReminder.GuildId },
                { "channel_Id", releaseReminder.ChannelId }
            }
        );

        List<string> usersToNotify = [];

        foreach (string userItem in releaseReminder.UserIdsToRemind)
        {
            usersToNotify.Add($"<@{userItem}>");
        }

        Album album = await _appleMusicApiService.GetAlbumFromCatalogAsync(releaseReminder.AlbumId);

        MusicEntityItem? musicEntityItem = await _odesliService.GetShareLinksAsync(album.Attributes!.Url);

        if (musicEntityItem is null)
        {
            _logger.LogWarning("Failed to get share links for album {AlbumId}. Skipping for now.", releaseReminder.AlbumId);

            activity?.SetStatus(ActivityStatusCode.Error);

            throw new NullReferenceException("Failed to get share links for album.");
        }

        using AlbumReleaseReminderResponse albumReleaseReminderResponse = new(album, musicEntityItem, usersToNotify);

        SocketTextChannel channel = guildItem.GetTextChannel(ulong.Parse(releaseReminder.ChannelId));

        if (channel is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error);

            releaseReminder.ReminderSent = true;
            
            throw new NullReferenceException("Failed to get channel.");
        }

        _logger.LogInformation("Sending album release reminder for album {AlbumId} to guild {GuildId}.", releaseReminder.AlbumId, guildItem.Id);

        FileAttachment fileAttachment = new(albumReleaseReminderResponse.AlbumArtworkStream, albumReleaseReminderResponse.AlbumArtFileName);

        try
        {
            await channel.SendFileAsync(
                embed: albumReleaseReminderResponse.GenerateEmbed().Build(),
                components: albumReleaseReminderResponse.GenerateComponent().Build(),
                attachment: fileAttachment,
                allowedMentions: AllowedMentions.All
            );
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);

            _logger.LogError(ex, "Failed to send album release reminder for album {AlbumId} to guild {GuildId}.", releaseReminder.AlbumId, guildItem.Id);

            albumReleaseReminderResponse.Dispose();

            throw new Exception("Failed to send album release reminder.", ex);
        }

        releaseReminder.ReminderSent = true;
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