using System.Diagnostics;
using Discord;

namespace MuzakBot.App.Extensions;

internal static class LyricsAnalyzerCommandModuleActivityExtensions
{
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