using System.Diagnostics;

namespace MuzakBot.App.Extensions;

internal static class GeniusApiServiceActivityExtensions
{
    public static Activity? StartGeniusSearchAsyncActivity(this ActivitySource activitySource, string artistName, string songName) => StartGeniusSearchAsyncActivity(activitySource, artistName, songName, null);
    public static Activity? StartGeniusSearchAsyncActivity(this ActivitySource activitySource, string artistName, string songName, string? parentActvitityId)
    {
        return activitySource.StartActivity(
            name: "GeniusSearchAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "artistName", artistName },
                { "songName", songName }
            },
            parentId: parentActvitityId
        );
    }

    public static Activity? StartGeniusGetLyricsAsyncActivity(this ActivitySource activitySource, string url) => StartGeniusGetLyricsAsyncActivity(activitySource, url, null);
    public static Activity? StartGeniusGetLyricsAsyncActivity(this ActivitySource activitySource, string url, string? parentActvitityId)
    {
        return activitySource.StartActivity(
            name: "GeniusGetLyricsAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "url", url }
            },
            parentId: parentActvitityId
        );
    }
}