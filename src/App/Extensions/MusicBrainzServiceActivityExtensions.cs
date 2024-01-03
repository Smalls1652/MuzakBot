using System.Diagnostics;
using MuzakBot.App.Models.Diagnostics;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ActivitySource"/> class related to the <see cref="MusicBrainzApiService"/>.
/// </summary>
internal static class MusicBrainzServiceActivityExtensions
{
    /// <summary>
    /// Starts an activity for the MusicBrainz API service.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="activityType">The type of activity.</param>
    /// <param name="tags">The activity tags.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartMusicBrainzServiceActivity(this ActivitySource activitySource, MusicBrainzActivityType activityType, MusicBrainzActivityTags tags) => StartMusicBrainzServiceActivity(activitySource, activityType, tags, null);

    /// <summary>
    /// Starts an activity for the MusicBrainz API service.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="activityType">The type of activity.</param>
    /// <param name="tags">The activity tags.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartMusicBrainzServiceActivity(this ActivitySource activitySource, MusicBrainzActivityType activityType, MusicBrainzActivityTags tags, string? parentActivityId)
    {
        string activityName = activityType switch
        {
            MusicBrainzActivityType.SearchArtist => "SearchArtistAsync",
            MusicBrainzActivityType.SearchArtistRecordings => "SearchArtistRecordingsAsync",
            MusicBrainzActivityType.SearchArtistReleases => "SearchArtistReleasesAsync",
            MusicBrainzActivityType.LookupArtist => "LookupArtistAsync",
            MusicBrainzActivityType.LookupRelease => "LookupReleaseAsync",
            MusicBrainzActivityType.LookupRecording => "LookupRecordingAsync",
            _ => throw new ArgumentOutOfRangeException(nameof(activityType), activityType, null)
        };

        return activitySource.StartActivity(
            name: activityName,
            kind: ActivityKind.Internal,
            tags: tags.ToActivityTagsCollection(),
            parentId: parentActivityId
        );
    }
}