using System.Diagnostics;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ActivitySource"/> class related to the <see cref="OdesliService"/>.
/// </summary>
internal static class OdesliServiceActivityExtensions
{
    /// <summary>
    /// Starts an activity to get share links for a given URL.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="inputUrl">The input URL.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartGetShareLinksActivity(this ActivitySource activitySource, string inputUrl) => StartGetShareLinksActivity(activitySource, inputUrl, null);

    /// <summary>
    /// Starts an activity to get share links for a given URL.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="inputUrl">The input URL.</param>
    /// <param name="parentActvitityId">The ID of the parent activity.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartGetShareLinksActivity(this ActivitySource activitySource, string inputUrl, string? parentActvitityId)
    {
        return activitySource.StartActivity(
            name: "GetShareLinksAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "inputUrl", inputUrl }
            },
            parentId: parentActvitityId
        );
    }
}