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

public sealed class AlbumReleaseReminderMonitorService : IAlbumReleaseReminderMonitorService
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _discordClient;
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly IOdesliService _odesliService;
    private readonly IDbContextFactory<AlbumReleaseDbContext> _albumReleaseDbContextFactory;
    private readonly TimeZoneInfo _easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public AlbumReleaseReminderMonitorService(ILogger<AlbumReleaseReminderMonitorService> logger, DiscordSocketClient discordClient, IAppleMusicApiService appleMusicApiService, IOdesliService odesliService, IDbContextFactory<AlbumReleaseDbContext> albumReleaseDbContextFactory)
    {
        _logger = logger;
        _discordClient = discordClient;
        _appleMusicApiService = appleMusicApiService;
        _odesliService = odesliService;
        _albumReleaseDbContextFactory = albumReleaseDbContextFactory;
    }

    public Task StartMonitorAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () => await ProcessAlbumReleaseRemindersAsync(cancellationToken), cancellationToken);
    }

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

                                continue;
                            }

                            AlbumReleaseReminderResponse albumReleaseReminderResponse = new(album, musicEntityItem, usersToNotify);

                            SocketTextChannel channel = guildItem.GetTextChannel(ulong.Parse(releaseReminder.ChannelId));

                            if (channel is null)
                            {
                                releaseReminder.ReminderSent = true;
                                continue;
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
                                _logger.LogError(ex, "Failed to send album release reminder for album {AlbumId} to guild {GuildId}.", releaseReminder.AlbumId, guildItem.Id);

                                albumReleaseReminderResponse.Dispose();

                                continue;
                            }

                            releaseReminder.ReminderSent = true;

                            albumReleaseReminderResponse.Dispose();
                        }
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    dbContext.Dispose();
                }
            }

            await Task.Delay(300000, cancellationToken);
        }

        _logger.LogInformation("Stopping album release reminder queue service.");
    }
}
