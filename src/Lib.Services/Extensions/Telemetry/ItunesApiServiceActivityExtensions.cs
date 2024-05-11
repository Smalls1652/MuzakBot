using System.Diagnostics;

using MuzakBot.Lib.Models.Diagnostics;

namespace MuzakBot.Lib.Services.Extensions.Telemetry;

/// <summary>
/// Provides extension methods for the <see cref="ActivitySource"/> class related to the <see cref="ItunesApiService"/>.
/// </summary>
internal static class ItunesApiServiceActivityExtensions
{
    /// <summary>
    /// Starts an activity for the iTunes API service.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="activityType">The type of activity.</param>
    /// <param name="tags">The activity tags.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartItunesApiServiceActivity(this ActivitySource activitySource, ItunesApiActivityType activityType, ItunesApiActivityTags tags) => StartItunesApiServiceActivity(activitySource, activityType, tags, null);

    /// <summary>
    /// Starts an activity for the iTunes API service.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="activityType">The type of activity.</param>
    /// <param name="tags">The activity tags.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartItunesApiServiceActivity(this ActivitySource activitySource, ItunesApiActivityType activityType, ItunesApiActivityTags tags, string? parentActivityId)
    {
        string activityName = activityType switch
        {
            ItunesApiActivityType.GetArtitstSearchResult => "GetArtitstSearchResultAsync",
            ItunesApiActivityType.GetArtistIdLookupResult => "GetArtistIdLookupResultAsync",
            ItunesApiActivityType.GetSongsByArtistResult => "GetSongsByArtistResultAsync",
            ItunesApiActivityType.GetAlbumsByArtistResult => "GetAlbumsByArtistResultAsync",
            ItunesApiActivityType.GetSongIdLookupResult => "GetSongIdLookupResultAsync",
            _ => throw new ArgumentException($"Unknown activity type: {activityType}", nameof(activityType))
        };

        return activitySource.StartActivity(
            name: activityName,
            kind: ActivityKind.Internal,
            tags: tags.ToActivityTagsCollection(),
            parentId: parentActivityId
        );
    }
}
