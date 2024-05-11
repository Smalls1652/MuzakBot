using System.Diagnostics;

namespace MuzakBot.Lib.Services.Extensions.Telemetry;

/// <summary>
/// Extensions for <see cref="ActivitySource"/> to start activities for <see cref="GeniusApiService"/>.
/// </summary>
internal static class GeniusApiServiceActivityExtensions
{
    /// <summary>
    /// Starts an activity for <see cref="GeniusApiService.SearchAsync(string, string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartGeniusSearchAsyncActivity(this ActivitySource activitySource, string artistName, string songName) => StartGeniusSearchAsyncActivity(activitySource, artistName, songName, null);

    /// <summary>
    /// Starts an activity for <see cref="GeniusApiService.SearchAsync(string, string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActvitityId">The ID of the parent activity (optional).</param>
    /// <returns>The started activity.</returns>
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

    /// <summary>
    /// Starts an activity for <see cref="GeniusApiService.GetLyricsAsync(string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="url">The URL of the web page.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartGeniusGetLyricsAsyncActivity(this ActivitySource activitySource, string url) => StartGeniusGetLyricsAsyncActivity(activitySource, url, null);

    /// <summary>
    /// Starts an activity for <see cref="GeniusApiService.GetLyricsAsync(string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="url">The URL of the web page.</param>
    /// <param name="parentActvitityId">The ID of the parent activity (optional).</param>
    /// <returns>The started activity.</returns>
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
