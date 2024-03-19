namespace MuzakBot.Lib.Models.Diagnostics;

/// <summary>
/// Represents the type of activity being performed by the <see cref="MusicBrainzApiService"/>.
/// </summary>
public enum MusicBrainzActivityType
{
    /// <summary>
    /// Represents the activity of running <see cref="MusicBrainzApiService.SearchArtistAsync(string)"/>.
    /// </summary>
    SearchArtist,

    /// <summary>
    /// Represents the activity of running <see cref="MusicBrainzApiService.SearchArtistRecordingsAsync(string)"/>.
    /// </summary>
    SearchArtistRecordings,

    /// <summary>
    /// Represents the activity of running <see cref="MusicBrainzApiService.SearchArtistReleasesAsync(string)"/>.
    /// </summary>
    SearchArtistReleases,

    /// <summary>
    /// Represents the activity of running <see cref="MusicBrainzApiService.LookupArtistAsync(string)"/>.
    /// </summary>
    LookupArtist,

    /// <summary>
    /// Represents the activity of running <see cref="MusicBrainzApiService.LookupReleaseAsync(string)"/>.
    /// </summary>
    LookupRelease,

    /// <summary>
    /// Represents the activity of running <see cref="MusicBrainzApiService.LookupRecordingAsync(string)"/>.
    /// </summary>
    LookupRecording
}