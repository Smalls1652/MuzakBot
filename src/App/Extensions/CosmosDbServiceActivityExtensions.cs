using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Extensions;

public static class CosmosDbServiceActivityExtensions
{
    public static Activity? StartDbAddOrUpdateSongLyricsItemActivity(this ActivitySource activitySource, SongLyricsItem songLyricsItem) => StartDbAddOrUpdateSongLyricsItemActivity(activitySource, songLyricsItem, null);

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

    public static Activity? StartDbGetSongLyricsItemActivity(this ActivitySource activitySource, string artistName, string songName) => StartDbGetSongLyricsItemActivity(activitySource, artistName, songName, null);

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