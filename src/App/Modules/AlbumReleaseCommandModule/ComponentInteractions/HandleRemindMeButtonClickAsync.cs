using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using MuzakBot.App.Models.Responses;
using MuzakBot.Database;
using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Database.AlbumRelease;

namespace MuzakBot.App.Modules;

public partial class AlbumReleaseCommandModule
{
    /// <summary>
    /// Handles the 'Remind me' button click.
    /// </summary>
    /// <param name="albumId">The ID of an album.</param>
    /// <returns></returns>
    [ComponentInteraction(
        customId: "albumrelease-remindme-*",
        ignoreGroupNames: true
    )]
    private async Task HandleRemindMeButtonClickAsync(string albumId)
    {
        var componentInteraction = (Context.Interaction as IComponentInteraction)!;

        await componentInteraction.DeferLoadingAsync(true);

        Album album = await _appleMusicApiService.GetAlbumFromCatalogAsync(albumId);

        DateOnly releaseDate = album.Attributes!.ReleaseDate;

        TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        DateTimeOffset releaseDateTime = new(
            year: releaseDate.Year,
            month: releaseDate.Month,
            day: releaseDate.Day,
            hour: 0,
            minute: 0,
            second: 0,
            offset: easternTimeZone.IsDaylightSavingTime(new(releaseDate.Year, releaseDate.Month, releaseDate.Day, 0, 0, 0))
                ? easternTimeZone.BaseUtcOffset.Add(new(1, 0, 0))
                : easternTimeZone.BaseUtcOffset
        );

        using AlbumReleaseDbContext dbContext = _albumReleaseDbContextFactory.CreateDbContext();

        AlbumReleaseReminder? albumReleaseReminder = dbContext.AlbumReleaseReminders
            .FirstOrDefault(item => item.Id == $"{album.Id}-{Context.Guild.Id}-{componentInteraction.ChannelId!.Value}");

        if (albumReleaseReminder is null)
        {
            albumReleaseReminder = new(
                albumId: album.Id,
                guildId: Context.Guild.Id.ToString(),
                channelId: componentInteraction.ChannelId!.Value.ToString(),
                releaseDate: releaseDateTime
            );

            dbContext.AlbumReleaseReminders.Add(albumReleaseReminder);
        }

        try
        {
            albumReleaseReminder.AddUserIdToRemind(Context.User.Id.ToString());
        }
        catch (InvalidOperationException)
        {
            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("You're already set to be reminded for this album!").Build(),
                ephemeral: true
            );

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add user ID to remind.");

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while adding you to be reminded.").Build(),
                ephemeral: true
            );

            return;
        }

        await dbContext.SaveChangesAsync();

        AlbumReleaseReminderAddedResponse reminderAddedResponse = new(
            album: album
        );

        await componentInteraction.FollowupAsync(
            embed: reminderAddedResponse.GenerateEmbed().Build(),
            ephemeral: true
        );
    }
}