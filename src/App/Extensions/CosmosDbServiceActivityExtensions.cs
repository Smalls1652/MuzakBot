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
}