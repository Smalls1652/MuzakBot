using MuzakBot.App.Models.MusicBrainz;

namespace MuzakBot.App.Services;

public interface IMusicBrainzService
{
    Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName);
    Task<MusicBrainzRecordingSearchResult?> SearchArtistRecordingsAsync(string artistId, string songName);
    Task<MusicBrainzArtistItem?> LookupArtistAsync(string artistId);
    Task<MusicBrainzReleaseItem?> LookupReleaseAsync(string releaseId);
    Task<MusicBrainzRecordingItem?> LookupRecordingAsync(string recordingId);
}