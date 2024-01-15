using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Extensions for <see cref="ActivitySource"/> to start activities for the <see cref="CosmosDbService"/>.
/// </summary>
public static class CosmosDbServiceActivityExtensions
{
    /// <summary>
    /// Starts an activity for adding or updating a song lyrics item in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="songLyricsItem">The song lyrics item.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateSongLyricsItemActivity(this ActivitySource activitySource, SongLyricsItem songLyricsItem) => StartDbAddOrUpdateSongLyricsItemActivity(activitySource, songLyricsItem, null);

    /// <summary>
    /// Starts an activity for adding or updating a song lyrics item in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="songLyricsItem">The song lyrics item.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateSongLyricsItemActivity(this ActivitySource activitySource, SongLyricsItem songLyricsItem, string? parentActivityId)
    {
        StringBuilder lyricsHashBuilder = new();

        using var hash = SHA256.Create();

        foreach (byte b in hash.ComputeHash(Encoding.UTF8.GetBytes(songLyricsItem.Lyrics)))
        {
            lyricsHashBuilder.Append(b.ToString("x2"));
        }

        return activitySource.StartActivity(
            name: "Database:AddOrUpdateSongLyricsItemAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "artistName", songLyricsItem.ArtistName },
                { "songName", songLyricsItem.SongName },
                { "lyricsHash", lyricsHashBuilder.ToString() }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for getting a song lyrics item from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetSongLyricsItemActivity(this ActivitySource activitySource, string artistName, string songName) => StartDbGetSongLyricsItemActivity(activitySource, artistName, songName, null);

    /// <summary>
    /// Starts an activity for getting a song lyrics item from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetSongLyricsItemActivity(this ActivitySource activitySource, string artistName, string songName, string? parentActivityId)
    {
        return activitySource.StartActivity(
            name: "Database:GetSongLyricsItemAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "artistName", artistName },
                { "songName", songName }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for getting a lyrics analyzer rate limit for a user from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetLyricsAnalyzerUserRateLimitActivity(this ActivitySource activitySource, ulong userId) => StartDbGetLyricsAnalyzerUserRateLimitActivity(activitySource, userId, null);

    /// <summary>
    /// Starts an activity for getting a lyrics analyzer rate limit for a user from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetLyricsAnalyzerUserRateLimitActivity(this ActivitySource activitySource, ulong userId, string? parentActivityId)
    {
        return activitySource.StartActivity(
            name: "Database:GetLyricsAnalyzerUserRateLimitAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "userId", userId.ToString() }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for adding or updating a lyrics analyzer rate limit for a user in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="lyricsAnalyzerUserRateLimit">The lyrics analyzer rate limit for the user.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateLyricsAnalyzerUserRateLimitActivity(this ActivitySource activitySource, LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit) => StartDbAddOrUpdateLyricsAnalyzerUserRateLimitActivity(activitySource, lyricsAnalyzerUserRateLimit, null);

    /// <summary>
    /// Starts an activity for adding or updating a lyrics analyzer rate limit for a user in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="lyricsAnalyzerUserRateLimit">The lyrics analyzer rate limit for the user.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateLyricsAnalyzerUserRateLimitActivity(this ActivitySource activitySource, LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit, string? parentActivityId)
    {
        return activitySource.StartActivity(
            name: "Database:AddOrUpdateLyricsAnalyzerUserRateLimitAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "userId", lyricsAnalyzerUserRateLimit.UserId.ToString() }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for getting the lyrics analyzer config from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetLyricsAnalyzerConfigActivity(this ActivitySource activitySource) => StartDbGetLyricsAnalyzerConfigActivity(activitySource, null);

    /// <summary>
    /// Starts an activity for getting the lyrics analyzer config from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetLyricsAnalyzerConfigActivity(this ActivitySource activitySource, string? parentActivityId)
    {
        return activitySource.StartActivity(
            name: "Database:GetLyricsAnalyzerConfigAsync",
            kind: ActivityKind.Internal,
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for adding or updating the lyrics analyzer config in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateLyricsAnalyzerConfigActivity(this ActivitySource activitySource) => StartDbAddOrUpdateLyricsAnalyzerConfigActivity(activitySource, null);

    /// <summary>
    /// Starts an activity for adding or updating the lyrics analyzer config in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateLyricsAnalyzerConfigActivity(this ActivitySource activitySource, string? parentActivityId)
    {
        return activitySource.StartActivity(
            name: "Database:AddOrUpdateLyricsAnalyzerConfigAsync",
            kind: ActivityKind.Internal,
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for adding or updating a lyrics analyzer prompt style in the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="promptStyle">The lyrics analyzer prompt style.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbAddOrUpdateLyricsAnalyzerPromptStyleActivity(this ActivitySource activitySource, LyricsAnalyzerPromptStyle promptStyle, string? parentActivityId)
    {
        return activitySource.CreateActivity(
            name: "Database:AddOrUpdateLyricsAnalyzerPromptStyleAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "db_Id", promptStyle.Id },
                { "name", promptStyle.Name },
                { "shortName", promptStyle.ShortName }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for getting a lyrics analyzer prompt style from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="shortName">The short name of the prompt style.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetLyricsAnalyzerPromptStyleActivity(this ActivitySource activitySource, string shortName, string? parentActivityId)
    {
        return activitySource.CreateActivity(
            name: "Database:GetLyricsAnalyzerPromptStyleAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "shortName", shortName }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Starts an activity for getting all lyrics analyzer prompt styles from the database.
    /// </summary>
    /// <param name="activitySource">The <see cref="ActivitySource"/>.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The started <see cref="Activity"/>.</returns>
    public static Activity? StartDbGetLyricsAnalyzerPromptStylesActivity(this ActivitySource activitySource, string? parentActivityId)
    {
        return activitySource.CreateActivity(
            name: "Database:GetLyricsAnalyzerPromptStylesAsync",
            kind: ActivityKind.Internal,
            parentId: parentActivityId
        );
    }
}