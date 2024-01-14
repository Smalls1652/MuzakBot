using MuzakBot.App.Models.MusicBrainz;

namespace MuzakBot.App.Services;

public interface IMusicBrainzService : IDisposable
{
    Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName);
    Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName, string? parentActivityId);

    Task<MusicBrainzRecordingSearchResult?> SearchArtistRecordingsAsync(string artistId, string songName);
    Task<MusicBrainzRecordingSearchResult?> SearchArtistRecordingsAsync(string artistId, string songName, string? parentActivityId);

    Task<MusicBrainzRecordingSearchResult?> SearchArtistByNameRecordingsAsync(string artistName, string songName);
    Task<MusicBrainzRecordingSearchResult?> SearchArtistByNameRecordingsAsync(string artistName, string songName, string? parentActivityId);

    Task<MusicBrainzReleaseSearchResult?> SearchArtistReleasesAsync(string artistId, string albumName);
    Task<MusicBrainzReleaseSearchResult?> SearchArtistReleasesAsync(string artistId, string albumName, string? parentActivityId);

    Task<MusicBrainzArtistItem?> LookupArtistAsync(string artistId);
    Task<MusicBrainzArtistItem?> LookupArtistAsync(string artistId, string? parentActivityId);

    Task<MusicBrainzReleaseItem?> LookupReleaseAsync(string releaseId);
    Task<MusicBrainzReleaseItem?> LookupReleaseAsync(string releaseId, string? parentActivityId);

    Task<MusicBrainzRecordingItem?> LookupRecordingAsync(string recordingId);
    Task<MusicBrainzRecordingItem?> LookupRecordingAsync(string recordingId, string? parentActivityId);
}