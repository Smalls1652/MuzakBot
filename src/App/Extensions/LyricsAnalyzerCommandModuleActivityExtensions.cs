using System.Diagnostics;
using Discord;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Extension methods for activity traces used by the <see cref="LyricsAnalyzerCommandModule"/> class.
/// </summary>
internal static class LyricsAnalyzerCommandModuleActivityExtensions
{
    /// <summary>
    /// Starts an activity for <see cref="LyricsAnalyzerCommandModule.HandleGetLyricsAsync(IInteractionContext, string, string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="songId">The ID of the song.</param>
    /// <param name="context">The <see cref="IInteractionContext"/> for the request.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartHandleGetLyricsAsyncActivity(this ActivitySource activitySource, string artistId, string songId, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleGetLyricsAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "SlashCommand"},
                { "command_Name", "getsonglyrics" },
                { "artist_Id", artistId },
                { "song_Id", songId },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }
}