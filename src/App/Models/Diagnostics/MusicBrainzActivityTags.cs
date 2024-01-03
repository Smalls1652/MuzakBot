using System.Diagnostics;

namespace MuzakBot.App.Models.Diagnostics;

/// <summary>
/// Represents the activity tags for a MusicBrainz entity.
/// </summary>
public class MusicBrainzActivityTags
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MusicBrainzActivityTags"/> class.
    /// </summary>
    public MusicBrainzActivityTags()
    {}

    /// <summary>
    /// Gets or sets the name of the artist.
    /// </summary>
    public string? ArtistName { get; set; }

    /// <summary>
    /// Gets or sets the ID of the artist.
    /// </summary>
    public string? ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the name of the song.
    /// </summary>
    public string? SongName { get; set; }

    /// <summary>
    /// Gets or sets the name of the album.
    /// </summary>
    public string? AlbumName { get; set; }

    /// <summary>
    /// Gets or sets the ID of the release.
    /// </summary>
    public string? ReleaseId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the recording.
    /// </summary>
    public string? RecordingId { get; set; }

    /// <summary>
    /// Converts the <see cref="MusicBrainzActivityTags"/> object to an <see cref="ActivityTagsCollection"/>.
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

        if (ReleaseId is not null)
        {
            tagsCollection.Add("releaseId", ReleaseId);
        }

        if (RecordingId is not null)
        {
            tagsCollection.Add("recordingId", RecordingId);
        }

        return tagsCollection;
    }
}