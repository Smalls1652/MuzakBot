using System.Diagnostics;

namespace MuzakBot.Lib.Models.Diagnostics;

/// <summary>
/// Represents the activity tags for the <see cref="ItunesApiService"/>.
/// </summary>
public class ItunesApiActivityTags
{
    /// <summary>
    /// Gets or sets the artist name.
    /// </summary>
    public string? ArtistName { get; set; }

    /// <summary>
    /// Gets or sets the artist ID.
    /// </summary>
    public string? ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the song name.
    /// </summary>
    public string? SongName { get; set; }

    /// <summary>
    /// Gets or sets the album name.
    /// </summary>
    public string? AlbumName { get; set; }

    /// <summary>
    /// Gets or sets the track ID.
    /// </summary>
    public string? TrackId { get; set; }

    /// <summary>
    /// Converts the <see cref="ItunesApiActivityTags"/> to an <see cref="ActivityTagsCollection"/>.
    /// </summary>
    /// <returns>The converted <see cref="ActivityTagsCollection"/>.</returns>
    public ActivityTagsCollection ToActivityTagsCollection()
    {
        ActivityTagsCollection tagsCollection = new();

        if (ArtistName is not null)
        {
            tagsCollection.Add("artistName", ArtistName);
        }

        if (ArtistId is not null)
        {
            tagsCollection.Add("artistId", ArtistId);
        }

        if (SongName is not null)
        {
            tagsCollection.Add("songName", SongName);
        }

        if (AlbumName is not null)
        {
            tagsCollection.Add("albumName", AlbumName);
        }

        if (TrackId is not null)
        {
            tagsCollection.Add("trackId", TrackId);
        }

        return tagsCollection;
    }
}