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
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="context">The <see cref="IInteractionContext"/> for the request.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartHandleGetLyricsAsyncActivity(this ActivitySource activitySource, string artistName, string songName, IInteractionContext context)
    {
        return activitySource.StartActivity(
            name: "HandleGetLyricsAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "SlashCommand"},
                { "command_Name", "getsonglyrics" },
                { "artist_Name", artistName },
                { "song_Name", songName },
                { "guild_Id", context.Guild.Id },
                { "guild_Name", context.Guild.Name },
                { "channel_Id", context.Channel.Id },
                { "channel_Name", context.Channel.Name }
            }
        );
    }
}