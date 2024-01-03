using System.Diagnostics;
using Discord;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ActivitySource"/> class related to the <see cref="ShareMusicCommandModule"/>.
/// </summary>
internal static class ShareMusicCommandModuleActivityExtensions
{
    /// <summary>
    /// Starts an activity for handling the asynchronous find album command.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="albumId">The ID of the album.</param>
    /// <param name="context">The interaction context.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartHandleFindAlbumAsyncActivity(this ActivitySource activitySource, string artistId, string albumId, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleFindAlbumAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "SlashCommand"},
                { "command_Name", "findalbum" },
                { "artist_Id", artistId },
                { "album_Id", albumId },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }

    /// <summary>
    /// Starts an activity for handling the asynchronous finding of a song.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="songId">The ID of the song.</param>
    /// <param name="context">The interaction context.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartHandleFindSongAsyncActivity(this ActivitySource activitySource, string artistId, string songId, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleFindSongAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "SlashCommand"},
                { "command_Name", "findsong" },
                { "artist_Id", artistId },
                { "song_Id", songId },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }

    /// <summary>
    /// Starts an activity for handling the asynchronous retrieval of music share links from a post.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="message">The message containing the post.</param>
    /// <param name="context">The interaction context.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartHandleGetLinksFromPostAsyncActivity(this ActivitySource activitySource, IMessage message, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleGetLinksFromPostAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "MessageCommand"},
                { "command_Name", "Get music share links" },
                { "message_Id", message.Id },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }

    /// <summary>
    /// Starts an activity for handling the asynchronous music share command.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="inputUrl">The input URL.</param>
    /// <param name="context">The interaction context.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartHandleMusicShareAsyncActivity(this ActivitySource activitySource, string inputUrl, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleMusicShareAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "SlashCommand"},
                { "command_Name", "sharemusic" },
                { "url", inputUrl },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }

    public static Activity? StartHandleMusicShareRefreshAsyncActivity(this ActivitySource activitySource, string url, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleMusicShareRefreshAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "ComponentInteraction"},
                { "command_Name", "Refresh music share links" },
                { "url", url },
                { "interaction_Id", context.Interaction.Id },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }
}